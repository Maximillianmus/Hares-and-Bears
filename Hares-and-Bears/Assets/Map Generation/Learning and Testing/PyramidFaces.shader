Shader "Custom/PyramidFacesCompute"
{
    Properties
    {
        _MainTex ("Texture", 2D) = "white" {}
    }
    SubShader
    {
        Tags { "RenderType"="Opaque" "RenderPipeline" = "UniversalPipeline"}

        Pass
        {
            Name "ForwardLit"
            Tags {"LightMode" = "UniversalForward"}

            HLSLPROGRAM
            //signals that this shader requires compute buffers
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 5.0

            #pragma multi_compile _ _MAIN_LIGHT_SHADOWS
            #pragma multi_compile _ _mAIN_LIGHT_SHADOWS_CASCADE
            #pragma multi_compile _ _ADDITIONAL_LIGHTS
            #pragma multi_compile _ _ADDITIONAL_LIGHTS_SHADOWS
            #pragma multi_compile _SHADOWS_SOFT

            #pragma vertex Vertex
            #pragma fragment Fragment

            #include "\Assets\Map Generation\Learning and Testing\PyramidFaces.hlsl"


            ENDHLSL
        }

        Pass
        {
            Name "ShadowCaster"
            Tags {"LightMode" = "ShadowCaster"}

            HLSLPROGRAM
            //signals that this shader requires compute buffers
            #pragma prefer_hlslcc gles
            #pragma exclude_renderers d3d11_9x
            #pragma target 5.0

            #pragma multi_compile_shadowcaster

            #pragma vertex Vertex
            #pragma fragment Fragment

            #define SHADOW_CASTER_PASS

            #include "\Assets\Map Generation\Learning and Testing\PyramidFaces.hlsl"

            ENDHLSL
        }
    }
}
