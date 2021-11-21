#ifndef PYRAMIDFACES_INCLUDED
#define PYRAMIDFACES_INCLUDED

#include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
#include "NMGPyramidGraphicsHelpers.hlsl"

struct DrawVertex
{
    float3 positionWS;
    float2 uv;
};

struct DrawTriangle
{
    float3 normalWS;
    DrawVertex vertices[3];
};


StructuredBuffer<DrawTriangle> _DrawTriangles;

struct VertexOutput
{
    float3 positionWS : TEXCOORD0;
    float3 normalWS : TEXCOORD1;
    float2 uv : TEXCOORD2;
    float4 positionCS : SV_POSITION;
};

TEXTURE2D(_MainTex); 
SAMPLER(sampler_MainTex); 
float4 _MainTex_ST;


VertexOutput Vertex(uint vertexID : SV_VertexID)
{
    VertexOutput output = (VertexOutput) 0;

    
    DrawTriangle tri = _DrawTriangles[vertexID / 3];
    DrawVertex input = tri.vertices[vertexID % 3];
    
    output.positionWS = input.positionWS;
    output.normalWS = tri.normalWS;
    output.uv = TRANSFORM_TEX(input.uv, _MainTex);
    
    output.positionCS = CalculatePositionCSWithShadowCasterLogic(input.positionWS, tri.normalWS);
    return output;

}

float4 Fragment(VertexOutput input) : SV_Target
{
#ifdef SHADOW_CASTER_PASS
    return 0;
#else
    InputData lightingInput = (InputData)0;
    lightingInput.positionWS = input.positionWS;
    lightingInput.normalWS = input.normalWS;
    lightingInput.viewDirectionWS = GetViewDirectionFromPosition(input.positionWS);
    lightingInput.shadowCoord = CalculatePositionCSWithShadowCasterLogic(input.positionWS, input.positionCS);


    float3 albedo = SAMPLE_TEXTURE2D(_MainTex, sampler_MainTex, input.uv).rgb;

    return UniversalFragmentBlinnPhong(lightingInput, albedo, 1, 0, 0, 1, 0);
#endif

}


#endif