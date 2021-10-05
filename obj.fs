
precision mediump float;

varying vec4 pos3D;
varying vec3 N;

#define M_PI 3.1415926535897932384626433832795

float sigma = 0.1;
float ni = 1.5;

vec3 SRCPOS = vec3(0,0,0);
vec3 SRCPOW = vec3(1,1,1);

float d_dot(vec3 a, vec3 b){
  return max(0.0, dot(a, b));
}

float Fresnel(vec3 i, vec3 m){
  float c = d_dot(i, m);
  float g = sqrt((ni*ni)+(c*c)-1.0);
	float n1 = (g-c)*(g-c);
	float n2 = (g+c)*(g+c);
	float m1 = (c*(g+c)-1.0)*(c*(g+c)-1.0);
	float m2 = (c*(g-c)+1.0)*(c*(g-c)+1.0);
  return n1;
}

float g(vec3 N, vec3 m, vec3 i, vec3 o){
	float n1 = 2.0*d_dot(N,m)*d_dot(N, o);
	n1 /= d_dot(o, m);
	float n2 = 2.0*d_dot(N,m)*d_dot(N, i);
	n2 /= d_dot(i, m);
  return min( min(1.0, n1), n2);
}

float beckmann (float cosTheta){
	float tang = tan(acos(cosTheta))*tan(acos(cosTheta));
	float n2 = (M_PI*sigma*sigma*(pow(cosTheta, 4.0)));
	float e = exp(-tang/(2.0*sigma*sigma));
  return (1.0/n2)*e;
}

float CookTorrance(vec3 N, vec3 m, vec3 i, vec3 o){
  float f = Fresnel(i, m) * beckmann(dot(N, m)) * g(N, m,i,o);
  //return f/(4.0*d_dot(i, N)*d_dot(o, N));
	//return beckmann(dot(N, m));
	return Fresnel(i, m);
	//return g(N, m,i,o);
}



void main(void)
{
  vec3 i = normalize(SRCPOS - pos3D.xyz);
  vec3 o = normalize(-pos3D.xyz);
  vec3 m = normalize(i + o);

	float cosTi = dot(N, o);

  float ct = CookTorrance(N, m,i,o);
	vec3 Fr = vec3(0.8,0.4,0.4) /*+ ct*/; // Lambert rendering, eye light source
	vec3 col = SRCPOW * Fr * cosTi;
	gl_FragColor = vec4(col,1.0);
}
