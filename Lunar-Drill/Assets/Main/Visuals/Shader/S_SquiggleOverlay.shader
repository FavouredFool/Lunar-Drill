Shader "Unlit/S_SquiggleOverlay"
{
    Properties
    {
        [NoScaleOffset] _MainTex("MainTex", 2D) = "white" {}
        _InverseSquiggleStrength("InverseSquiggleStrength", Float) = 500
        _SquiggleFrequency("SquiggleFrequency", Float) = 3
        _FPS("FPS", Float) = 12
        [HideInInspector][NoScaleOffset]unity_Lightmaps("unity_Lightmaps", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_LightmapsInd("unity_LightmapsInd", 2DArray) = "" {}
        [HideInInspector][NoScaleOffset]unity_ShadowMasks("unity_ShadowMasks", 2DArray) = "" {}
    }
        SubShader
    {
        Tags
        {
            "RenderPipeline" = "UniversalPipeline"
            "RenderType" = "Transparent"
            "UniversalMaterialType" = "Unlit"
            "Queue" = "Transparent"
        // DisableBatching: <None>
        "ShaderGraphShader" = "true"
        "ShaderGraphTargetId" = "UniversalSpriteUnlitSubTarget"
    }
    Pass
    {
        Name "Sprite Unlit"
        Tags
        {
            "LightMode" = "Universal2D"
        }

        // Render State
        Cull Off
        Blend SrcAlpha OneMinusSrcAlpha, One OneMinusSrcAlpha
        ZTest LEqual
        ZWrite Off

        // Debug
        // <None>

        // --------------------------------------------------
        // Pass

        HLSLPROGRAM

        // Pragmas
        #pragma target 2.0
        #pragma exclude_renderers d3d11_9x
        #pragma vertex vert
        #pragma fragment frag

        // Keywords
        #pragma multi_compile_fragment _ DEBUG_DISPLAY
        // GraphKeywords: <None>

        // Defines

        #define ATTRIBUTES_NEED_NORMAL
        #define ATTRIBUTES_NEED_TANGENT
        #define ATTRIBUTES_NEED_TEXCOORD0
        #define ATTRIBUTES_NEED_COLOR
        #define VARYINGS_NEED_POSITION_WS
        #define VARYINGS_NEED_TEXCOORD0
        #define VARYINGS_NEED_COLOR
        #define FEATURES_GRAPH_VERTEX
        /* WARNING: $splice Could not find named fragment 'PassInstancing' */
        #define SHADERPASS SHADERPASS_SPRITEUNLIT


        // custom interpolator pre-include
        /* WARNING: $splice Could not find named fragment 'sgci_CustomInterpolatorPreInclude' */

        // Includes
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Color.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Texture.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Core.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Lighting.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/Input.hlsl"
        #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/TextureStack.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/ShaderGraphFunctions.hlsl"
        #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/ShaderPass.hlsl"

        // --------------------------------------------------
        // Structs and Packing

        // custom interpolators pre packing
        /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPrePacking' */

        struct Attributes
        {
             float3 positionOS : POSITION;
             float3 normalOS : NORMAL;
             float4 tangentOS : TANGENT;
             float4 uv0 : TEXCOORD0;
             float4 color : COLOR;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : INSTANCEID_SEMANTIC;
            #endif
        };
        struct Varyings
        {
             float4 positionCS : SV_POSITION;
             float3 positionWS;
             float4 texCoord0;
             float4 color;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };
        struct SurfaceDescriptionInputs
        {
             float4 uv0;
             float3 TimeParameters;
        };
        struct VertexDescriptionInputs
        {
             float3 ObjectSpaceNormal;
             float3 ObjectSpaceTangent;
             float3 ObjectSpacePosition;
        };
        struct PackedVaryings
        {
             float4 positionCS : SV_POSITION;
             float4 texCoord0 : INTERP0;
             float4 color : INTERP1;
             float3 positionWS : INTERP2;
            #if UNITY_ANY_INSTANCING_ENABLED
             uint instanceID : CUSTOM_INSTANCE_ID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
             uint stereoTargetEyeIndexAsBlendIdx0 : BLENDINDICES0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
             uint stereoTargetEyeIndexAsRTArrayIdx : SV_RenderTargetArrayIndex;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
             FRONT_FACE_TYPE cullFace : FRONT_FACE_SEMANTIC;
            #endif
        };

        PackedVaryings PackVaryings(Varyings input)
        {
            PackedVaryings output;
            ZERO_INITIALIZE(PackedVaryings, output);
            output.positionCS = input.positionCS;
            output.texCoord0.xyzw = input.texCoord0;
            output.color.xyzw = input.color;
            output.positionWS.xyz = input.positionWS;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }

        Varyings UnpackVaryings(PackedVaryings input)
        {
            Varyings output;
            output.positionCS = input.positionCS;
            output.texCoord0 = input.texCoord0.xyzw;
            output.color = input.color.xyzw;
            output.positionWS = input.positionWS.xyz;
            #if UNITY_ANY_INSTANCING_ENABLED
            output.instanceID = input.instanceID;
            #endif
            #if (defined(UNITY_STEREO_MULTIVIEW_ENABLED)) || (defined(UNITY_STEREO_INSTANCING_ENABLED) && (defined(SHADER_API_GLES3) || defined(SHADER_API_GLCORE)))
            output.stereoTargetEyeIndexAsBlendIdx0 = input.stereoTargetEyeIndexAsBlendIdx0;
            #endif
            #if (defined(UNITY_STEREO_INSTANCING_ENABLED))
            output.stereoTargetEyeIndexAsRTArrayIdx = input.stereoTargetEyeIndexAsRTArrayIdx;
            #endif
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            output.cullFace = input.cullFace;
            #endif
            return output;
        }


        // --------------------------------------------------
        // Graph

        // Graph Properties
        CBUFFER_START(UnityPerMaterial)
        float4 _MainTex_TexelSize;
        float _InverseSquiggleStrength;
        float _SquiggleFrequency;
        float _FPS;
        CBUFFER_END


            // Object and Global properties
            SAMPLER(SamplerState_Linear_Repeat);
            TEXTURE2D(_MainTex);
            SAMPLER(sampler_MainTex);

            // Graph Includes
            #include "Packages/com.unity.render-pipelines.core/ShaderLibrary/Hashes.hlsl"

            // -- Property used by ScenePickingPass
            #ifdef SCENEPICKINGPASS
            float4 _SelectionID;
            #endif

            // -- Properties used by SceneSelectionPass
            #ifdef SCENESELECTIONPASS
            int _ObjectId;
            int _PassValue;
            #endif

            // Graph Functions

            void Unity_Multiply_float_float(float A, float B, out float Out)
            {
                Out = A * B;
            }

            void Unity_Round_float(float In, out float Out)
            {
                Out = round(In);
            }

            void Unity_Add_float4(float4 A, float4 B, out float4 Out)
            {
                Out = A + B;
            }

            float2 Unity_Voronoi_RandomVector_Deterministic_float(float2 UV, float offset)
            {
                Hash_Tchou_2_2_float(UV, UV);
                return float2(sin(UV.y * offset), cos(UV.x * offset)) * 0.5 + 0.5;
            }

            void Unity_Voronoi_Deterministic_float(float2 UV, float AngleOffset, float CellDensity, out float Out, out float Cells)
            {
                float2 g = floor(UV * CellDensity);
                float2 f = frac(UV * CellDensity);
                float t = 8.0;
                float3 res = float3(8.0, 0.0, 0.0);
                for (int y = -1; y <= 1; y++)
                {
                    for (int x = -1; x <= 1; x++)
                    {
                        float2 lattice = float2(x, y);
                        float2 offset = Unity_Voronoi_RandomVector_Deterministic_float(lattice + g, AngleOffset);
                        float d = distance(lattice + offset, f);
                        if (d < res.x)
                        {
                            res = float3(d, offset.x, offset.y);
                            Out = res.x;
                            Cells = res.y;
                        }
                    }
                }
            }

            void Unity_Remap_float(float In, float2 InMinMax, float2 OutMinMax, out float Out)
            {
                Out = OutMinMax.x + (In - InMinMax.x) * (OutMinMax.y - OutMinMax.x) / (InMinMax.y - InMinMax.x);
            }

            void Unity_Divide_float(float A, float B, out float Out)
            {
                Out = A / B;
            }

            // Custom interpolators pre vertex
            /* WARNING: $splice Could not find named fragment 'CustomInterpolatorPreVertex' */

            // Graph Vertex
            struct VertexDescription
            {
                float3 Position;
                float3 Normal;
                float3 Tangent;
            };

            VertexDescription VertexDescriptionFunction(VertexDescriptionInputs IN)
            {
                VertexDescription description = (VertexDescription)0;
                description.Position = IN.ObjectSpacePosition;
                description.Normal = IN.ObjectSpaceNormal;
                description.Tangent = IN.ObjectSpaceTangent;
                return description;
            }

            // Custom interpolators, pre surface
            #ifdef FEATURES_GRAPH_VERTEX
            Varyings CustomInterpolatorPassThroughFunc(inout Varyings output, VertexDescription input)
            {
            return output;
            }
            #define CUSTOMINTERPOLATOR_VARYPASSTHROUGH_FUNC
            #endif

            // Graph Pixel
            struct SurfaceDescription
            {
                float3 BaseColor;
                float Alpha;
            };

            SurfaceDescription SurfaceDescriptionFunction(SurfaceDescriptionInputs IN)
            {
                SurfaceDescription surface = (SurfaceDescription)0;
                UnityTexture2D _Property_b9f4b096ac5140e2a53a17c0419136b6_Out_0_Texture2D = UnityBuildTexture2DStructNoScale(_MainTex);
                float4 _UV_a50b3a42a50e41d5a7376764449e3093_Out_0_Vector4 = IN.uv0;
                float _Property_9bc7c2fd4bca4bc0a9e860647df7856c_Out_0_Float = _FPS;
                float _Multiply_eb84f1b1813449b4ba444acf279d2a15_Out_2_Float;
                Unity_Multiply_float_float(_Property_9bc7c2fd4bca4bc0a9e860647df7856c_Out_0_Float, IN.TimeParameters.x, _Multiply_eb84f1b1813449b4ba444acf279d2a15_Out_2_Float);
                float _Round_0bf23559dd324f8bbaaeba85141b3441_Out_1_Float;
                Unity_Round_float(_Multiply_eb84f1b1813449b4ba444acf279d2a15_Out_2_Float, _Round_0bf23559dd324f8bbaaeba85141b3441_Out_1_Float);
                float _Multiply_11724688d133498aba2f319ac919fabf_Out_2_Float;
                Unity_Multiply_float_float(2, _Round_0bf23559dd324f8bbaaeba85141b3441_Out_1_Float, _Multiply_11724688d133498aba2f319ac919fabf_Out_2_Float);
                float4 _Add_940a56ed4e9a4785a4d0a60109070454_Out_2_Vector4;
                Unity_Add_float4(_UV_a50b3a42a50e41d5a7376764449e3093_Out_0_Vector4, (_Multiply_11724688d133498aba2f319ac919fabf_Out_2_Float.xxxx), _Add_940a56ed4e9a4785a4d0a60109070454_Out_2_Vector4);
                float _Multiply_685e5496749c4d2d8cb0002739497254_Out_2_Float;
                Unity_Multiply_float_float(_Round_0bf23559dd324f8bbaaeba85141b3441_Out_1_Float, 0.1, _Multiply_685e5496749c4d2d8cb0002739497254_Out_2_Float);
                float _Property_70c95e12c1c3464bb235fbde3bab1a42_Out_0_Float = _SquiggleFrequency;
                float _Voronoi_d71222c4a3554c62be9de7756c00f764_Out_3_Float;
                float _Voronoi_d71222c4a3554c62be9de7756c00f764_Cells_4_Float;
                Unity_Voronoi_Deterministic_float((_Add_940a56ed4e9a4785a4d0a60109070454_Out_2_Vector4.xy), _Multiply_685e5496749c4d2d8cb0002739497254_Out_2_Float, _Property_70c95e12c1c3464bb235fbde3bab1a42_Out_0_Float, _Voronoi_d71222c4a3554c62be9de7756c00f764_Out_3_Float, _Voronoi_d71222c4a3554c62be9de7756c00f764_Cells_4_Float);
                float _Remap_ce066b52ccf648d98758153780506be2_Out_3_Float;
                Unity_Remap_float(_Voronoi_d71222c4a3554c62be9de7756c00f764_Out_3_Float, float2 (0, 1), float2 (-1, 1), _Remap_ce066b52ccf648d98758153780506be2_Out_3_Float);
                float _Property_20b16306c5cc467abea24f8cc4537bc5_Out_0_Float = _InverseSquiggleStrength;
                float _Divide_0aaba9d47fbc49448bb7dfa9e8774d1a_Out_2_Float;
                Unity_Divide_float(_Remap_ce066b52ccf648d98758153780506be2_Out_3_Float, _Property_20b16306c5cc467abea24f8cc4537bc5_Out_0_Float, _Divide_0aaba9d47fbc49448bb7dfa9e8774d1a_Out_2_Float);
                float4 _Add_8f717df1a545459ba22ea5f2ddb9ab01_Out_2_Vector4;
                Unity_Add_float4(_UV_a50b3a42a50e41d5a7376764449e3093_Out_0_Vector4, (_Divide_0aaba9d47fbc49448bb7dfa9e8774d1a_Out_2_Float.xxxx), _Add_8f717df1a545459ba22ea5f2ddb9ab01_Out_2_Vector4);
                float4 _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4 = SAMPLE_TEXTURE2D(_Property_b9f4b096ac5140e2a53a17c0419136b6_Out_0_Texture2D.tex, _Property_b9f4b096ac5140e2a53a17c0419136b6_Out_0_Texture2D.samplerstate, _Property_b9f4b096ac5140e2a53a17c0419136b6_Out_0_Texture2D.GetTransformedUV((_Add_8f717df1a545459ba22ea5f2ddb9ab01_Out_2_Vector4.xy)));
                float _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_R_4_Float = _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4.r;
                float _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_G_5_Float = _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4.g;
                float _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_B_6_Float = _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4.b;
                float _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_A_7_Float = _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4.a;
                float _Split_986983ea0a2a4426bf2465296f89e1be_R_1_Float = _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4[0];
                float _Split_986983ea0a2a4426bf2465296f89e1be_G_2_Float = _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4[1];
                float _Split_986983ea0a2a4426bf2465296f89e1be_B_3_Float = _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4[2];
                float _Split_986983ea0a2a4426bf2465296f89e1be_A_4_Float = _SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4[3];
                surface.BaseColor = (_SampleTexture2D_3befa1164e68460cb29bde4b661b3fef_RGBA_0_Vector4.xyz);
                surface.Alpha = _Split_986983ea0a2a4426bf2465296f89e1be_A_4_Float;
                return surface;
            }

            // --------------------------------------------------
            // Build Graph Inputs
            #ifdef HAVE_VFX_MODIFICATION
            #define VFX_SRP_ATTRIBUTES Attributes
            #define VFX_SRP_VARYINGS Varyings
            #define VFX_SRP_SURFACE_INPUTS SurfaceDescriptionInputs
            #endif
            VertexDescriptionInputs BuildVertexDescriptionInputs(Attributes input)
            {
                VertexDescriptionInputs output;
                ZERO_INITIALIZE(VertexDescriptionInputs, output);

                output.ObjectSpaceNormal = input.normalOS;
                output.ObjectSpaceTangent = input.tangentOS.xyz;
                output.ObjectSpacePosition = input.positionOS;

                return output;
            }
            SurfaceDescriptionInputs BuildSurfaceDescriptionInputs(Varyings input)
            {
                SurfaceDescriptionInputs output;
                ZERO_INITIALIZE(SurfaceDescriptionInputs, output);

            #ifdef HAVE_VFX_MODIFICATION
            #if VFX_USE_GRAPH_VALUES
                uint instanceActiveIndex = asuint(UNITY_ACCESS_INSTANCED_PROP(PerInstance, _InstanceActiveIndex));
                /* WARNING: $splice Could not find named fragment 'VFXLoadGraphValues' */
            #endif
                /* WARNING: $splice Could not find named fragment 'VFXSetFragInputs' */

            #endif








                #if UNITY_UV_STARTS_AT_TOP
                #else
                #endif


                output.uv0 = input.texCoord0;
                output.TimeParameters = _TimeParameters.xyz; // This is mainly for LW as HD overwrite this value
            #if defined(SHADER_STAGE_FRAGMENT) && defined(VARYINGS_NEED_CULLFACE)
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN output.FaceSign =                    IS_FRONT_VFACE(input.cullFace, true, false);
            #else
            #define BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN
            #endif
            #undef BUILD_SURFACE_DESCRIPTION_INPUTS_OUTPUT_FACESIGN

                    return output;
            }

            // --------------------------------------------------
            // Main

            #include "Packages/com.unity.render-pipelines.universal/Editor/ShaderGraph/Includes/Varyings.hlsl"
            #include "Packages/com.unity.render-pipelines.universal/Editor/2D/ShaderGraph/Includes/SpriteUnlitPass.hlsl"

            // --------------------------------------------------
            // Visual Effect Vertex Invocations
            #ifdef HAVE_VFX_MODIFICATION
            #include "Packages/com.unity.render-pipelines.universal/ShaderLibrary/VisualEffectVertex.hlsl"
            #endif

            ENDHLSL
            }
         
    }
        CustomEditor "UnityEditor.ShaderGraph.GenericShaderGraphMaterialGUI"
                                        FallBack "Hidden/Shader Graph/FallbackError"
}