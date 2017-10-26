# NavMeshDetourSharp
c# warpper for easy use , run at server or u3d android. with test example  
这个是用recastnavigation / recastnavigation 的包装.提取了几个常用的接口方便c#测试调用.  
最初是用来解决linux服务器寻路问题,但是无法方便的可视化,于是用c#写了一个测试工具观看怪物的行为是否正确  
  
使用方法:  
Warpper dh = new Warpper();  
succ = dh.Load(bytesFileName);//load map  
if (dh.FindPath(true, start, end, path)) xxxxx //get path  
..
  
自己编译的时候,要注意x86跟x64匹配问题,都可以用  
  
上图:  
<img src='https://github.com/a11s/NavMeshDetourSharp/raw/master/libdetour/demo/images/findpath.png'/>
