Shader "Stencil Group/Unstencil"
{
	SubShader
	{
		Tags { "Queue"="Geometry-2" }
		Zwrite Off
		ColorMask 0

		Stencil
		{
			Ref 2
			Pass replace
		}


			Pass
			{
			}
	}
}
