修改此项目class1中的加载dll路径
编译此项目,在CAD加载此dll

1.若你的是net framework项目,在其.csproj 中添加
<PropertyGroup>
  <Deterministic>False</Deterministic>
</PropertyGroup>

然后要修改你的项目中 AssemblyInfo.cs
[assembly: AssemblyVersion("1.0.*")]

2.若你的是net standard项目,增加.csproj文件,没有就自己创建一个
不是net standard项目,则不需要
<PropertyGroup>
    <AssemblyVersion>1.0.0.*</AssemblyVersion> 
    <FileVersion>1.0.0.0</FileVersion>
    <Deterministic>False</Deterministic>
</PropertyGroup>


cad加载此dll,用此dll的 ww 命令,就会动态加载你的dll到CAD
不能用其他方式加载你的dll

这样你的dll就可以不用频繁的开关AutoCAD,进行修改,编译,加载dll