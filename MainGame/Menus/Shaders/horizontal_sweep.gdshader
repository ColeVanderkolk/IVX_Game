shader_type canvas_item;

uniform float progress: hint_range(0,1);
uniform bool fill = true;

// Called for every pixel the material is visible on.
void fragment() {
	if (UV.x > progress) {
		if (fill) {
			discard;
		}
	} else if (!fill) {
		discard;
	}
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
