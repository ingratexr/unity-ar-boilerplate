Shader "Stencil Group/Inside Stencil" 
{
	Properties
	{
		[Enum(Equal,3,NotEqual,6)] _StencilTest ("Stencil Test", int) = 3
	}

	  SubShader {
		// draw after all opaque objects (queue = 2001):
		Tags { "Queue"="Geometry-1" }
		
		Stencil{
			Ref 1
			Comp[_StencilTest]
		}
	
		Pass {
		  Blend Zero One // keep the image behind it
		}
	  } 
}