Shader "Custom/StencilGeometry"
{
	Properties
	{
		_StencilMask("Stencil Mask", Int) = 0
	}
	Category
	{
		Tags { "Queue"="Geometry" }

		Lighting Off

		BindChannels
		{
			Bind "Color", color
			Bind "Vertex", vertex
			Bind "TexCoord", texcoord
		}

		SubShader 
		{
			Tags { "RenderType"="Opaque" }
			LOD 200

			Stencil
			{
				Ref[_StencilMask]
				Comp Equal
				Pass replace
			}

			Pass
			{

			}
		}
	}
}