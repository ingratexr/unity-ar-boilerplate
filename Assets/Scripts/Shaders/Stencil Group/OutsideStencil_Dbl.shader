Shader "Stencil Group/Outside Stencil (Dbl Sided)"
{
	SubShader
	{
		Tags { "Queue"="Geometry-2" }
		Cull Off
		Zwrite off
		ColorMask 0

		Stencil
		{
			Ref 1
			Pass replace
		}


			Pass
			{
			}
	}
}
