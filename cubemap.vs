attribute vec3 a_position;

varying vec3 v_position;

uniform mat4 uMVMatrix;
uniform mat4 uPMatrix;

void main() {
  v_position =  a_position;
  gl_Position = uPMatrix * uMVMatrix * vec4(a_position * 50.0, 1.0);
  //gl_Position.z = 1.0;

}
