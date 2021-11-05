
precision mediump float;

varying vec4 pos3D;
varying vec3 N;
varying mat4 rotMatrix;

uniform samplerCube u_skybox;

uniform float material;
uniform float sigma;

#define M_PI 3.1415926535897932384626433832795


float ni = 4.0;

vec3 SRCPOS = vec3(0);
vec3 SRCPOW = vec3(2);

// ============================================================

float d_dot(vec3 a, vec3 b){
  return max(0.0, dot(a, b));
}


// ============================================================

float Fresnel(float im){
  float c = im;
  float g = sqrt((ni*ni)+(c*c)-1.0);
	float n1 = (g-c)*(g-c);
	float n2 = (g+c)*(g+c);
	float m1 = (c*(g+c)-1.0)*(c*(g+c)-1.0);
	float m2 = (c*(g-c)+1.0)*(c*(g-c)+1.0);
  return 0.5*(n1/n2)*(1.0+(m1/m2));
}

// ============================================================

float g(float Nm, float No, float om, float Ni, float im){
	float n1 = (2.0*Nm*No)/om;
	float n2 = (2.0*Nm*Ni)/im;
  return min( min(1.0, n1), n2);
}

// ============================================================

float beckmann (float cosTheta){
	float tang = tan(acos(cosTheta))*tan(acos(cosTheta));
  if(tang < 0.0) return 0.0;
	float denom = (M_PI*sigma*sigma*(pow(cosTheta, 4.0)));
	float e = exp(-tang/(2.0*sigma*sigma));
  return (e/denom);
}

// ============================================================

float CookTorrance(float Nm, float No, float om, float Ni, float im){
  float FDG = Fresnel(im) * beckmann(Nm) * g(Nm, No, om, Ni, im);
  return FDG/(4.0*Ni*No);
}

// ============================================================

void main(void)
{

  vec3 i = normalize(SRCPOS - pos3D.xyz);
  vec3 o = normalize(-pos3D.xyz); // o == obsPos - pos3D.xyz
  vec3 m = normalize(i + o);

  float cosTi = d_dot(N, i);

  float cosTm = d_dot(N, m);
  float cosTo = d_dot(N, o);
  float om = d_dot(o, m);
  float im = d_dot(i, m);


  // ========== Cook-Torrance ===========

  if(material == 0.0) {
    float FrSpec = CookTorrance(cosTm, cosTo, om, cosTi, im);
  	vec3 Fr = (vec3(0.9,0.1,0.1)/M_PI) + FrSpec*0.1; // Lambert rendering, eye light source
  	vec3 col = SRCPOW * Fr * cosTi;

    gl_FragColor = vec4(col, 1.0);
  }



  // ========== Mirror material ===========


  if(material == 1.0){
    vec3 refl = reflect(-o, N);
    vec4 reflCol = textureCube(u_skybox, normalize(refl * mat3(rotMatrix)));
    gl_FragColor = reflCol;
  }



  // ========== Transparent material ===========

  if(material == 2.0){
    float ior = 1.0/1.3;
    vec3 refr = refract(-o, N, ior);
    vec3 refl = reflect(-o, N);


    float R2 = Fresnel(im);
    vec4 refrCol = textureCube(u_skybox, normalize(refr * mat3(rotMatrix)));
    vec4 reflCol = textureCube(u_skybox, normalize(refl * mat3(rotMatrix)));
    gl_FragColor = (1.0 - R2) * refrCol + R2 * reflCol;
  }



}
