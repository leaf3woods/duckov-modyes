# 鸭科夫MOD计划

### 自定义基地BGM ✅ 已完成



### 0. 局内效果

<center>
   <img src="doc\modbgm.png" alt="局内效果预览">
</center>

### 1. 加载模式

同时支持官方提供的加载方式，及bepinex的加载方式

mod老手建议使用bepinex



#### 2. 文件夹结构

* 创意工坊用户点击订阅即可

* mod目录如下所示，手动安装的话请放入Mods文件夹内，并确认文件结构如下图所示

```shell
D:.[游戏Mods目录]
└─moyes-custom-basebgm
    │  0Harmony.dll
    │  BepInEx.dll
    │  CustomBaseBgm.dll
    │  info.ini
    │  preview.png
    │
    └─MyBGM
            d1v - kush ballad.flac
```



### 3. 使用说明

* 初次加载会在moyes-custom-basebgm目录下生成一个MyBGM文件夹，把你希望替换的背景音乐放入该文件夹

* 目前支持：.mp3,  .flac,  .aac,  .m4a 类型的音频文件，放入其他类型的文件会被排除

* 如果目录下没有能加载的音乐文件，会重新使用系统自带的背景音乐
  
  

### 4. 其他

[GitHub - pardeike/Harmony: A library for patching, replacing and decorating .NET and Mono methods during runtime](https://github.com/pardeike/Harmony)

https://github.com/BepInEx/BepInEx[GitHub - BepInEx/BepInEx: Unity / XNA game patcher and plugin framework](https://github.com/BepInEx/BepInEx)


