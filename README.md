asmwalk
=======

Console app to walk an assembly's references 

```
asmwalk ILSpy.exe

+ILSpy 2.3.2.1942 (v4.0.30319)
 +ICSharpCode.AvalonEdit 5.0.3.0 (v4.0.30319)
 +ICSharpCode.TreeView 4.2.0.8752 (v4.0.30319)
 +ICSharpCode.Decompiler 2.3.2.1942 (v4.0.30319)
  +Mono.Cecil 0.9.6.0 (v4.0.30319)
  +ICSharpCode.NRefactory 5.0.0.0 (v4.0.30319)
  +ICSharpCode.NRefactory.CSharp 5.0.0.0 (v4.0.30319)
   -ICSharpCode.NRefactory 5.0.0.0 (again)
 -Mono.Cecil 0.9.6.0 (again)
 -ICSharpCode.NRefactory.CSharp 5.0.0.0 (again)
 -ICSharpCode.NRefactory 5.0.0.0 (again)
 +ICSharpCode.NRefactory.VB 5.0.0.0 (v4.0.30319)
  -ICSharpCode.NRefactory 5.0.0.0 (again)
  -ICSharpCode.NRefactory.CSharp 5.0.0.0 (again)
 +Mono.Cecil.Pdb 0.9.6.0 (v4.0.30319)
  -Mono.Cecil 0.9.6.0 (again)
  ```
