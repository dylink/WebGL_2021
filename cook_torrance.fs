
precision mediump float;

varying vec4 pos3D;
varying vec3 N;

#define M_PI 3.1415926535897932384626433832795

float sigma = 0.5;
float ni = 1.5;

vec3 SRCPOS = vec3(5,5,5);
vec3 SRCPOW = vec3(1,1,1);

float d_dot(vec3 a, vec3 b){
  return max(0, dot(a, b));
}

vec3 Fresnel(vec3 i, vec3 m){
  float c = d_dot(i, m);
  float g = sqrt(ni*ni+c*c-1);
  return (0.5*(((g-c)*(g-c))/((g+c)*(g+c)))*(1+(pow(c*(g+c)-1, 2))/(pow(c*(g-c)+1, 2)));
}

vec3 g(vec3 m, vec3 i, vec3 o){
  min(min(1, (2*d_dot(N, m)*d_dot(N,o))/dot(o,m)), (2*d_dot(N, m)*d_dot(N,i))/d_dot(i,m));
}

float beckmann (float theta){
  return (1/(M_PI*pow(cos(theta), 4)))*exp(-(tan(theta)*tan(theta))/2*sigma*sigma);
}

vec3 CookTorrance(vec3 m, vec3 i, vec3 o){
  vec3 f = Fresnel(i, m) * beckmann(dot(N, m)) * g(m,i,o);
  return f/(4*d_dot(i, N)*d_dot(o, N));
}



void main(void)
{
  vec3 i = normalize(SRCPOS - pos3D.xyz);
  vec3 o = normalize(-pos3D.xyz);
  vec3 m = normalize(i + o);
  vec3 ct = CookTorrance(m,i,o);
	vec3 col = ct * vec3(0.8,0.4,0.4) * dot(N,normalize(vec3(-pos3D))); // Lambert rendering, eye light source
	gl_FragColor = vec4(col,1.0);
}
