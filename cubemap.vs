attribute vec3 a_position;

varying vec3 v_position;

uniform mat4 uMVMatrix;
uniform mat4 uPMatrix;

void main() {
  v_position =  a_position;
  gl_Position = uPMatrix * uMVMatrix * vec4(a_position * 40.0, 1.0);

}
