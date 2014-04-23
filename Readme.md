# WpfGL

This project demonstrates how to (more or less) efficiently render OpenGL content in a way that can be used in a Windows Presentation Foundation project without Airspace issues. This means  full support for transparency and composition on top of the render image.

This demo also contains an example of how to do multisampling (anti-aliasing) to avoid hard edges on the render result.


## Theory 

The problem: 
- Wpf WriteableBitmap prefers Pbgra colors (premultiplied with alpha), and has 0,0 at the upper left corner.  
- OpenGL textures have 0,0 at the lower left corner and don't know about premultiplied colors.

My approach is therefore as follows:

1. All OpenGL content is rendered into a texture. 
1. This texture is post-processed using a shader that inverts the rows and pre-multiplies the color values with the alpha value. 
1. The resulting image is read back in an asynchronous way using pixel buffer objects.
1. The pixel buffer object is mapped and copied into a WriteableBitmap.

The post-processing step is applied in a shader pass because my tests showed that this is the fastest way. 

## License

Licensed under the simplified BSD license.

 