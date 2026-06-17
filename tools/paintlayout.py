#!/usr/bin/env python3
"""
paintlayout.py - helper for Hybrid Animals paint brushes (ColorScheme data files).

Two modes:

  --mode layout   (default)  build a PaintLayout<mesh>_<layout> line for an item file.
  --mode colors              just list every swatch in the brush with its hex color, so you
                             know which value paints which color before you assign them.

Background
----------
PaintableObject.LoadLayoutData reads an item-file key

    PaintLayout<meshIndex>_<layoutName>      (interior models: PaintLayoutINTERIOR<meshIndex>_<layoutName>)

where <layoutName> is the brush's  [Layout] value= .  The value is a list of
"<materialIndex>-<paint_material>" pairs, e.g.  "0-1, 1-2, 2-3, 3-4, ".

ColorizeMesh resolves each material's swatch by matching:

    swatch.palette_index == paint_material - 1       (paint_material 0 => swatch "None" => BLACK)

So the usable paint_material values for a brush are (palette_index + 1) of its swatches,
skipping swatches whose color is 0,0,0 (those paint black too).

Examples
--------
  # list every swatch + hex for a brush
  python paintlayout.py --mode colors --paint-path "PaintBrushData/Weapon - Magma.txt"

  # build a layout line for a mesh whose materials go 0..3 in the Unity inspector
  python paintlayout.py --paint-path "Weapon - Magma.txt" --material-amount 3

  # same, but force material 0 to a specific swatch (by name, by value, or by nearest hex)
  python paintlayout.py --paint-path "Weapon - Magma.txt" --material-amount 3 \
        --set 0=Col4 --set 1=6 --set 2=#79696a

  # interior model, second painted mesh
  python paintlayout.py --paint-path "Weapon - Magma.txt" --material-amount 3 --mesh 1 --interior
"""

from __future__ import annotations

import argparse
import sys
from pathlib import Path


# ----------------------------------------------------------------------------- parsing

def parse_scheme(text: str):
    """Parse the INI-ish brush file into (layout_name, [swatch, ...])."""
    layout_name = None
    swatches = []          # {section, palette_index, color|None}
    section = None
    data: dict[str, str] = {}

    def flush(sec, d):
        nonlocal layout_name
        if sec is None:
            return
        if sec == "Layout":
            layout_name = d.get("value")
            return
        if "palette_index" in d:
            try:
                idx = int(d["palette_index"])
            except ValueError:
                return
            swatches.append({
                "section": sec,
                "palette_index": idx,
                "color": parse_color(d.get("color")),
            })

    for raw in text.splitlines():
        line = raw.strip()
        if not line:
            continue
        if line.startswith("[") and line.endswith("]"):
            flush(section, data)
            section, data = line[1:-1], {}
            continue
        if "=" in line:
            k, _, v = line.partition("=")
            data[k.strip()] = v.strip()
    flush(section, data)
    return layout_name, swatches


def parse_color(s):
    if not s:
        return None
    try:
        parts = [float(p) for p in s.split(",")]
    except ValueError:
        return None
    return parts[:3] if len(parts) >= 3 else None


def is_black(color, eps: float = 1e-4) -> bool:
    return color is not None and all(c <= eps for c in color[:3])


def to_hex(color) -> str:
    if color is None:
        return "--------"
    r, g, b = (max(0, min(255, round(c * 255))) for c in color[:3])
    return f"#{r:02X}{g:02X}{b:02X}"


def value_of(sw) -> int:
    """The paint_material value you write in the layout for this swatch (palette_index + 1)."""
    return sw["palette_index"] + 1


# ----------------------------------------------------------------------------- resolving overrides

def resolve_spec(spec: str, swatches):
    """
    Turn an override spec into a paint_material value.
      - "#RRGGBB"   -> nearest non-black swatch by color
      - "<name>"    -> that swatch section (e.g. Col4)
      - "<int>"     -> used verbatim as the paint_material value
    """
    spec = spec.strip()
    if spec.startswith("#"):
        h = spec.lstrip("#")
        if len(h) != 6:
            raise ValueError(f"bad hex '{spec}' (want #RRGGBB)")
        target = [int(h[i:i + 2], 16) / 255.0 for i in (0, 2, 4)]
        candidates = [s for s in swatches if s["color"] is not None and not is_black(s["color"])]
        if not candidates:
            raise ValueError("no colored swatches to match a hex against")
        best = min(candidates, key=lambda s: sum((a - b) ** 2 for a, b in zip(s["color"], target)))
        return value_of(best)

    by_name = {s["section"].lower(): s for s in swatches}
    if spec.lower() in by_name:
        return value_of(by_name[spec.lower()])

    try:
        return int(spec)
    except ValueError:
        raise ValueError(f"'{spec}' is not a hex (#RRGGBB), a swatch name, or an integer value")


def swatch_for_value(value: int, swatches):
    """The swatch a paint_material value maps to (palette_index == value-1), or None."""
    for s in swatches:
        if s["palette_index"] == value - 1:
            return s
    return None


# ----------------------------------------------------------------------------- modes

def mode_colors(brush, layout_name, swatches) -> int:
    print(f"Paint brush : {brush}")
    print(f"Layout name : {layout_name or '(none)'}")
    print()
    if not swatches:
        print("No swatches (sections with palette_index=) found.")
        return 1
    swatches.sort(key=lambda s: s["palette_index"])
    print(f"  {'value':>5}  {'idx':>3}  {'hex':<8}  {'section':<16}  rgb(0-1)")
    for s in swatches:
        col = s["color"]
        rgb = "(" + ", ".join(f"{c:.3f}" for c in col) + ")" if col else "(no color)"
        tag = "  <- black" if is_black(col) else ""
        print(f"  {value_of(s):>5}  {s['palette_index']:>3}  {to_hex(col):<8}  {s['section']:<16}  {rgb}{tag}")
    print()
    print("Use the 'value' column in --mode layout (e.g. --set 0=<value>), or 0 for unpainted.")
    return 0


def mode_layout(args, brush, layout_name, swatches) -> int:
    print(f"Paint brush : {brush}")
    if not layout_name:
        print("error: no [Layout] value= in this file - items cannot map swatches to it.", file=sys.stderr)
        return 1
    print(f"Layout name : {layout_name}")

    if args.material_amount is None:
        print("error: --material-amount is required in layout mode "
              "(the highest material index shown in the Unity inspector, 0-based).", file=sys.stderr)
        return 2
    if args.material_amount < 0:
        print("error: --material-amount cannot be negative.", file=sys.stderr)
        return 2

    n_slots = args.material_amount + 1           # 0-based index -> count
    prefix = "PaintLayoutINTERIOR" if args.interior else "PaintLayout"
    key = f"{prefix}{args.mesh}_{layout_name}"

    usable = [s for s in swatches if not is_black(s["color"])]
    if not usable:
        print("error: every swatch is black (0,0,0); this brush paints black no matter what.", file=sys.stderr)
        return 1
    usable_vals = [value_of(s) for s in usable]

    # explicit overrides
    overrides: dict[int, int] = {}
    for raw in (args.set or []):
        if "=" not in raw:
            print(f"error: --set '{raw}' must be INDEX=SPEC (e.g. 0=Col4, 1=6, 2=#79696a).", file=sys.stderr)
            return 2
        idx_s, _, spec = raw.partition("=")
        try:
            idx = int(idx_s)
        except ValueError:
            print(f"error: --set index '{idx_s}' is not an integer.", file=sys.stderr)
            return 2
        if not (0 <= idx <= args.material_amount):
            print(f"error: --set index {idx} is outside 0..{args.material_amount}.", file=sys.stderr)
            return 2
        try:
            overrides[idx] = resolve_spec(spec, swatches)
        except ValueError as e:
            print(f"error: --set {raw}: {e}", file=sys.stderr)
            return 2

    # build, cycling usable swatches for any slot not overridden
    assignments = []
    for slot in range(n_slots):
        val = overrides.get(slot, usable_vals[slot % len(usable_vals)])
        assignments.append((slot, val))

    line = ", ".join(f"{slot}-{val}" for slot, val in assignments) + ", "

    print(f"Item key    : {key}")
    print(f"Materials   : index 0..{args.material_amount} ({n_slots} slot(s))")
    print()
    print(f"  {'material':>8}  {'value':>5}  {'hex':<8}  swatch")
    for slot, val in assignments:
        sw = swatch_for_value(val, swatches)
        name = sw["section"] if sw else ("None/unpainted" if val == 0 else "<no swatch for this value>")
        hexc = to_hex(sw["color"]) if sw else "--------"
        flag = "  (override)" if slot in overrides else ""
        warn = "  <- BLACK" if (sw and is_black(sw["color"])) or val == 0 else ""
        print(f"  {slot:>8}  {val:>5}  {hexc:<8}  {name}{flag}{warn}")
    print()
    print("Layout line (item .txt):")
    print(f"  {key} = {line}")
    print()
    print("HAModHelper ExtraFields entry:")
    print(f'  ["{key}"] = "{line}",')
    return 0


# ----------------------------------------------------------------------------- entry

def main(argv=None) -> int:
    ap = argparse.ArgumentParser(
        description="Build/inspect Hybrid Animals paint layouts for a paint brush.",
        formatter_class=argparse.RawDescriptionHelpFormatter)
    ap.add_argument("--paint-path", required=True, metavar="PATH",
                    help="Path to a PaintBrushData/<name>.txt brush file")
    ap.add_argument("--mode", choices=("layout", "colors"), default="layout",
                    help="layout = build a PaintLayout line (default); colors = list swatch hex codes")
    ap.add_argument("--material-amount", type=int, default=None, metavar="INDEX",
                    help="layout mode: highest material index as shown in the Unity inspector "
                         "(0-based). A mesh with 4 material slots (Element 0..3) is --material-amount 3.")
    ap.add_argument("--set", action="append", metavar="INDEX=SPEC", default=None,
                    help="layout mode: force a material. SPEC = swatch name (Col4), a value (6), "
                         "or a hex (#79696a -> nearest swatch). Repeatable.")
    ap.add_argument("--mesh", type=int, default=0, metavar="I",
                    help="target_meshes index for the key (default 0)")
    ap.add_argument("--interior", action="store_true",
                    help="use the PaintLayoutINTERIOR... key (PaintableObject.is_interior_model == true)")
    args = ap.parse_args(argv)

    path = Path(args.paint_path)
    if not path.is_file():
        print(f"error: file not found: {path}", file=sys.stderr)
        return 2

    layout_name, swatches = parse_scheme(path.read_text(encoding="utf-8", errors="replace"))
    brush = path.stem

    if args.mode == "colors":
        return mode_colors(brush, layout_name, swatches)
    return mode_layout(args, brush, layout_name, swatches)


if __name__ == "__main__":
    raise SystemExit(main())
