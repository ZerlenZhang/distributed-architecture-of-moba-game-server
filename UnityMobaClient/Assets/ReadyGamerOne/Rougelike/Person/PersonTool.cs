using System.IO;
using ReadyGamerOne.Global;
using ReadyGamerOne.Utility;
using UnityEditor;
using UnityEngine;
using FileUtil = ReadyGamerOne.Utility.FileUtil;

namespace ReadyGamerOne.Rougelike.Person
{
//#pragma warning disable CS0414
    public class PersonTool
#if UNITY_EDITOR
        :IEditorTools
#endif
    {
#if UNITY_EDITOR
        static string Title = "角色系统";
        private static string sourDir;
        private static string generateDir;
        private static bool createControllers = true;

        static void OnToolsGUI(string rootNs, string viewNs, string constNs, string dataNs, string autoDir,
            string scriptDir)
        {
               EditorGUILayout.Space();
               
               EditorGUILayout.HelpBox("使用本工具需要满足以下条件：\n" +
                                       "1、源预制体目录在Resources下\n" +
                                       "2、需要单独分类的角色放在Type前缀目录下\n" +
                                       "3、需要有和分类名对应的Data类型",MessageType.Info);
   
               EditorGUILayout.Space();
               
               EditorGUILayout.LabelField("角色预制体目录", sourDir);
               if (GUILayout.Button("编辑角色预制体路径"))
               {
                   sourDir = EditorUtility.OpenFolderPanel("选择预制体目录", Application.dataPath, "");
               }
   
               EditorGUILayout.Space();
               
               EditorGUILayout.LabelField("角色类命名空间",rootNs+".Model.Person");
               createControllers = EditorGUILayout.Toggle("是否生成对应控制器", createControllers);
               
               EditorGUILayout.Space();
   
               if (GUILayout.Button("开始生成", GUILayout.Height(3 * EditorGUIUtility.singleLineHeight)))
               {
                   if (string.IsNullOrEmpty(sourDir))
                   {
                       Debug.LogError("预制体目录不能为空");
                       return;
                   }
                   if (!sourDir.Contains("Resources"))
                   {
                       Debug.LogError("预制体目录需要在Resources下");
                       return;
                   }
                   
                   StartGenerate( rootNs, createControllers);
               }
         
        }

        private static string rootNs;

        private static void StartGenerate(string rootNs, bool createControllers)
        {
            PersonTool.rootNs = rootNs;
            

            
            FileUtil.SearchDirectory(
                sourDir,
                OnOperateFile,
                true,
                OnOperateFolder);

            

            AssetDatabase.Refresh();
            Debug.Log("生成完毕");
        }

        private static bool OnOperateFolder(DirectoryInfo dirInfo)
        {

//            if (dirInfo.FullName!=sourDir && dirInfo.Name.StartsWith("Type") == false)
//                return false;

            var curName = dirInfo.Name;
            var parName = dirInfo.Parent.Name;

            var parControllerName = "";
            var controllerName = "";
            if ( curName == FileUtil.GetDirName(sourDir))
            {
                parName = "PoolDataPerson";
                curName = "Person";
                controllerName = rootNs + curName + "Controller";
                parControllerName = "AbstractPersonController";
            }
            else if(parName == FileUtil.GetDirName(sourDir))
            {
                parName = rootNs + "Person";
                curName = curName.GetAfterSubstring("Type");
                
                
                controllerName = rootNs + curName + "Controller";
                parControllerName = parName + "Controller";
            }
            else
            {
                parName = rootNs + parName.GetAfterSubstring("Type");
                curName = curName.GetAfterSubstring("Type");
                
                controllerName = rootNs + curName + "Controller";
                parControllerName = parName + "Controller";
            }
            
            var genDir = Application.dataPath + "/" + rootNs + "/Model/Person" + dirInfo.GetSpecialPath("Type")+"/"+rootNs+curName;
            
            CreateAbstractPersonType(genDir,dirInfo,curName,parName);
            CreateAbstractControllerType(genDir, curName, controllerName, parControllerName);
            return true;
        }

        private static void OnOperateFile(FileInfo fileInfo)
        {
            if(fileInfo.Name.EndsWith(".meta"))
                return;
            var fileName = Path.GetFileNameWithoutExtension(fileInfo.FullName);
            var genPath = Application.dataPath + "/" + rootNs + "/Model/Person" +
                          fileInfo.Directory.GetSpecialPath("Type") + "/" + fileName;
            var parentName = rootNs + fileInfo.Directory.Name.GetAfterSubstring("Type");

            CreatePersonType(genPath, fileName, parentName);
            CreateControllerType(genPath, fileName + "Controller", parentName + "Controller");
//
//            Debug.Log(genPath+"\n" +
//                      parentName+"\n" +
//                      fileInfo.Name);
        }

        private static void CreateControllerType(string gendir, string className, string parentName)
        {
            FileUtil.CreateFolder(gendir);
            
            var fullpath = gendir + "/" + className + ".cs";
            if (File.Exists(fullpath))
                return;
            
           
            var stream = File.CreateText(fullpath); 
            
            stream.Write("namespace "+rootNs+".Model.Person\n" +
                         "{\n" +
                         "\tpublic class "+className+" :\n" +
                         "\t\t"+parentName+"\n" +
                         "\t{\n" +
                         "\t\tpublic override float MoveSpeed { get; set; }\n" +
                         "\t\tpublic override void SetMoveable(bool state)\n" +
                         "\t\t{\n" +
                         "\t\t\tthrow new System.NotImplementedException();\n" +
                         "\t\t}\n" +
                         "\t}\n" +
                         "}\n");
            
            
            stream.Dispose();
            stream.Close();
        }

        private static void CreatePersonType(string gendir, string className, string parentName)
        {
            FileUtil.CreateFolder(gendir);

            var fullpath = gendir + "/" + className + ".cs";
            if (File.Exists(fullpath))
                return;

            var stream = File.CreateText(fullpath);

            stream.Write("using ReadyGamerOne.Data;\n" +
                         "using ReadyGamerOne.Rougelike.Person;\n" +
                         "using " + rootNs + ".Const;\n" +
                         "namespace " + rootNs + ".Model.Person\n" +
                         "{\n");
            if(createControllers)
                stream.Write("\t[UsePersonController(typeof("+className+"Controller))]\n"); 
            stream.Write("\tpublic partial class "+className+" :\n" +
                         "\t\t"+parentName+"<"+className+">\n" +
                         "\t{\n" +
                         "\t\tpublic override string ResPath => PersonPath."+className+";\n" +
                         "\t\tpublic override void LoadData(CsvMgr data)\n" +
                         "\t\t{\n" +
                         "\t\t\t//    在这里需要使用数据对类内变量进行初始化赋值(比如_hp,_maxhp,_attack，这三项自动生成在基类中了)\n" +
                         "\t\t\t//    这个函数人物每次激活时都会调用\n" +
                         "\t\t}\n" +
                         "\t}\n" +
                         "}\n");
            
            stream.Dispose();
            stream.Close();
        }
        
        private static void CreateAbstractControllerType(string gendir,string curName, string controllerName, string parControllerName)
        {
            FileUtil.CreateClassFile(
                controllerName,
                rootNs + ".Model.Person",
                gendir,
                parControllerName,
                curName+"角色控制类，在角色类加UsePersonController属性的话会自动添加上去，使用InitController初始化",
                usingStatements:"using ReadyGamerOne.Rougelike.Person;\n",
                isAbstract:true);            
        }

        private static void CreateAbstractPersonType(string gendir, DirectoryInfo dirInfo, string curName,string parName)
        {
            var className = rootNs + curName;

            var isRootPerson = new DirectoryInfo(dirInfo.FullName).FullName == new DirectoryInfo(sourDir).FullName;
            
            FileUtil.CreateFolder(gendir);

            var fileFullPath = gendir + "/" + className + ".cs";
            if (File.Exists(fileFullPath))
                return;
            
            var stream = File.CreateText(fileFullPath);

            stream.Write("using ReadyGamerOne.Rougelike.Person;\n");
            
            
            if (dirInfo.HasValuableFiles())
                stream.Write("using System;\n" +
                             "using "+rootNs+".Data;\n");


            stream.Write("using UnityEngine;\n\n" +
                         "namespace " + rootNs + ".Model.Person\n" +
                         "{\n" +
                         "\tpublic interface I" + className);

            if(!isRootPerson)
                stream.Write(" : \n" +
                             "\t\tI"+parName);
            stream.Write("\n" +
                         "\t{\n" +
                         "\t}\n\n" +
                         "\tpublic abstract class " + className + "<T>:\n" +
                         "\t\t" + parName + "<T>,\n" +
                         "\t\tI" + className + "\n" +
                         "\t\twhere T : " + className + "<T>,new()\n" +
                         "\t{\n");
            
            if(isRootPerson)
                stream.Write("\t\t#region Fields\n\n" +
                             "\t\tprotected int _hp;\n" +
                             "\t\tprotected int _maxHp;\n" +
                             "\t\tprotected int _attack;\n\n" +
                             "\t\t#endregion\n\n" +
                             "\t\t#region ITakeDamageablePerson<T>\n\n" +
                             "\t\tpublic override int Hp => _hp;\n" +
                             "\t\tpublic override int MaxHp => _maxHp;\n" +
                             "\t\tpublic virtual void OnTakeDamage(AbstractPerson takeDamageFrom, int damage)\n" +
                             "\t\t{\n" +
                             "\t\t\tDebug.Log($\"{CharacterName}收到来自{takeDamageFrom.CharacterName}的{damage}伤害\");\n\n" +
                             "\t\t\t_hp -= damage;\n" +
                             "\t\t\tif(_hp<0)\n" +
                             "\t\t\t\tKill();\n" +
                             "\t\t}\n\n" +
                             "\t\t public void OnCauseDamage(AbstractPerson causeDamageTo, int damage)\n" +
                             "\t\t{\n" +
                             "\t\t\tDebug.Log($\"{CharacterName}对{causeDamageTo.CharacterName}造成{damage}伤害\");\n" +
                             "\t\t}\n\n" +
                             "\t\t#endregion\n");
            
            if (dirInfo.HasValuableFiles())
                stream.Write("\t\tpublic override Type DataType => typeof(" + curName + "Data);\n");
            
            stream.Write("\t}\n" +
                         "}\n");
            stream.Dispose();
            stream.Close();            
        }
#endif
    }
    
    
}