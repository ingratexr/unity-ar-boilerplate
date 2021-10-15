Shader "Stencil Group/Outside Stencil"
{
	SubShader
	{
		Tags { "Queue"="Geometry-2" }
		Zwrite off
		ColorMask 0

		Stencil
		{
			Ref 1
			Pass replace
			Zfail DecrWrap
		}


			Pass
			{
			}
	}
}
