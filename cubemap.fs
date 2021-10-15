precision mediump float;

uniform samplerCube u_skybox;

varying vec3 v_position;

void main() {
  gl_FragColor = textureCube(u_skybox, v_position);
	//gl_FragColor = vec4(0);
}
