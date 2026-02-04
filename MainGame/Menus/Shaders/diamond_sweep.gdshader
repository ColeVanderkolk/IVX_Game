shader_type canvas_item;

uniform float progress: hint_range(0,1);
uniform bool fill = true;
uniform float size = 40.0;

// Called for every pixel the material is visible on.
void fragment() {
	float x = abs(fract(FRAGCOORD.x / size) - 0.5);
	float y = abs(fract(FRAGCOORD.y / size) - 0.5);
	
	if (x + y + UV.x > progress * 2.0) {
		if (fill){
			discard;
		}
	} else if (!fill) {
		discard;
	}
}
