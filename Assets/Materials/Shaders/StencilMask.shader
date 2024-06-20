Shader "Custom/StencilMask"
{
	Properties
	{
		_StencilMask("Stencil Mask", Int) = 0
	}
	SubShader
	{
		Tags
		{
			"Queue" = "Geometry-100"
		}

		ColorMask 0
		ZWrite Off

		Pass
		{
			Stencil
			{
				Ref[_StencilMask]
				Comp Always
				Pass replace
				Fail zero
			}			
		}
	}
}