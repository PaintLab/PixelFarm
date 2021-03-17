//AUTOGEN, 2020-11-26T21:57:10
//source: D:\projects\PixelFarm\src\x_resource_projects\TestAtlas1\TestAtlas1.csproj
//tools: D:\projects\PixelFarm\src\Tools\PaintLabResTool\bin\Debug\PaintLabResTool.exe
using PixelFarm.Drawing;
namespace Atlas_AUTOGEN_.TestAtlas1{
public partial class Binders{
public readonly AtlasImageBinder _arrow_blank_png=new AtlasImageBinder(RawAtlasData.NAME, "//arrow_blank.png");
public readonly AtlasImageBinder _arrow_close_png=new AtlasImageBinder(RawAtlasData.NAME, "//arrow_close.png");
public readonly AtlasImageBinder _arrow_open_png=new AtlasImageBinder(RawAtlasData.NAME, "//arrow_open.png");
public readonly AtlasImageBinder _check_buttons_png=new AtlasImageBinder(RawAtlasData.NAME, "//check_buttons.png");
public readonly AtlasImageBinder _chk_checked_png=new AtlasImageBinder(RawAtlasData.NAME, "//chk_checked.png");
public readonly AtlasImageBinder _chk_unchecked_png=new AtlasImageBinder(RawAtlasData.NAME, "//chk_unchecked.png");
public readonly AtlasImageBinder _drop_down_button_png=new AtlasImageBinder(RawAtlasData.NAME, "//drop_down_button.png");
public readonly AtlasImageBinder _favorites32_png=new AtlasImageBinder(RawAtlasData.NAME, "//favorites32.png");
public readonly AtlasImageBinder _html32_png=new AtlasImageBinder(RawAtlasData.NAME, "//html32.png");
public readonly AtlasImageBinder _opt_checked_png=new AtlasImageBinder(RawAtlasData.NAME, "//opt_checked.png");
public readonly AtlasImageBinder _opt_unchecked_png=new AtlasImageBinder(RawAtlasData.NAME, "//opt_unchecked.png");


                    static bool s_registered;
                    public Binders(){
                            if(!s_registered){        
                                    try{
                                         PixelFarm.Platforms.InMemStorage.AddData(RawAtlasData.NAME + ".info", RawAtlasData.info);
                                         PixelFarm.Platforms.InMemStorage.AddData(RawAtlasData.NAME + ".png", RawAtlasData.img);
                                    }catch(System.Exception ex){
                                    }
                            s_registered= true;
                            }
                    }
                
}
}
