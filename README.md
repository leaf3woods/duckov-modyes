# 鸭科夫MOD计划

### 自定义基地BGM ✅ 已完成

#### [本项目已在github开源：leaf3woods/duckov-modyes](https://github.com/leaf3woods/duckov-modyes)

### 0. 局内效果

<center>
   <img src="doc\modbgm.png" alt="局内效果预览">
</center>

### 1. 加载模式

支持官方及bepinex的加载方式



#### 2. 文件夹结构

* 创意工坊用户点击订阅即可

* 手动安装的话请放入[游戏Mods目录] 下的Mods文件夹内

* 使用BepinEX 时 目录如下

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

* 使用官方加载mod时目录如下所示，音乐放在游戏exe所在文件夹的MyBGM里

```shell
D:.[游戏Mods目录]
└─moyes-custom-basebgm
       0Harmony.dll
       BepInEx.dll
       CustomBaseBgm.dll
       info.ini
       preview.png
```



### 3. 使用说明

* 初次加载会自动生成一个MyBGM文件夹，把你希望替换的背景音乐放入该文件夹中

* 目前支持：.mp3,  .flac,  .aac,  .m4a 类型的音频文件，放入其他类型的文件会被排除

* 如果目录下没有能加载的音乐文件，会重新使用系统自带的背景音乐
  
  

### 4. 其他

[GitHub - pardeike/Harmony: A library for patching, replacing and decorating .NET and Mono methods during runtime](https://github.com/pardeike/Harmony)

https://github.com/BepInEx/BepInEx[GitHub - BepInEx/BepInEx: Unity / XNA game patcher and plugin framework](https://github.com/BepInEx/BepInEx)


