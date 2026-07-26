shader_type canvas_item;

uniform float lightness_curve = 1.0f;
uniform float effect = 1.0f;

void vertex() {
	// Called for every vertex the material is visible on.
}

void fragment() {
	float lightness = (COLOR.r + COLOR.g + COLOR.b) / 3.0f;
	lightness = pow(lightness, lightness_curve);
	COLOR.rgb = mix(COLOR.rgb, vec3(1,1,1)*lightness, effect);
	// Called for every pixel the material is visible on.
}

//void light() {
//	// Called for every pixel for every light affecting the CanvasItem.
//	// Uncomment to replace the default light processing function with this one.
//}
