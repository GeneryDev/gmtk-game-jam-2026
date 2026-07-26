shader_type canvas_item;

varying vec4 modulate;

void vertex() {
	modulate = COLOR;
}

vec3 rgb_to_hsv(vec3 rgb) {
    float max_c = max(rgb.r, max(rgb.g, rgb.b));
    float min_c = min(rgb.r, min(rgb.g, rgb.b));
    float delta = max_c - min_c;
    
    vec3 hsv = vec3(0.0);
    hsv.z = max_c;
    hsv.y = max_c == 0.0 ? 0.0 : delta / max_c;
    
    if (delta != 0.0) {
        if (max_c == rgb.r) {
            hsv.x = mod((rgb.g - rgb.b) / delta, 6.0);
        } else if (max_c == rgb.g) {
            hsv.x = (rgb.b - rgb.r) / delta + 2.0;
        } else {
            hsv.x = (rgb.r - rgb.g) / delta + 4.0;
        }
        hsv.x = hsv.x / 6.0;
    }
    
    return hsv;
}

vec3 hsv_to_rgb(vec3 hsv) {
    float c = hsv.z * hsv.y;
    float h = hsv.x * 6.0;
    float x = c * (1.0 - abs(mod(h, 2.0) - 1.0));
    float m = hsv.z - c;
    
    vec3 rgb;
    if (h < 1.0) {
        rgb = vec3(c, x, 0.0);
    } else if (h < 2.0) {
        rgb = vec3(x, c, 0.0);
    } else if (h < 3.0) {
        rgb = vec3(0.0, c, x);
    } else if (h < 4.0) {
        rgb = vec3(0.0, x, c);
    } else if (h < 5.0) {
        rgb = vec3(x, 0.0, c);
    } else {
        rgb = vec3(c, 0.0, x);
    }
    
    return rgb + m;
}

void fragment() {
	
	float hue_shift = (modulate.r - 0.5f) * 360.0f - 180.0f;
	float saturation = 1.0f;
	float brightness = modulate.b + 1.0f;
	
    vec4 tex_color = texture(TEXTURE, UV);
    vec3 rgb = tex_color.rgb;
    
    vec3 hsv = rgb_to_hsv(rgb);
    hsv.x = mod(hsv.x + hue_shift / 360.0, 1.0);
    hsv.y *= saturation;
    hsv.y = clamp(hsv.y, 0.0, 1.0);
    hsv.z *= brightness;
    hsv.z = clamp(hsv.z, 0.0, 1.0);
    
    rgb = hsv_to_rgb(hsv);
    
    COLOR = vec4(rgb, tex_color.a * modulate.a);
}