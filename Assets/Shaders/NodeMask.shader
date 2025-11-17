Shader "Custom/StencilMask"
{
    SubShader
    {
        Tags { "Queue"="Geometry-10" }   // render early
        ColorMask 0                       // draw no color
        ZWrite Off

        Stencil {
            Ref 1
            Comp Always
            Pass Replace
        }

        Pass {}
    }
}