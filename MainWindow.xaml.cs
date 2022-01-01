using System;
using System.Diagnostics;
using System.IO;
using System.Windows;
using ICSharpCode.SharpZipLib.Zip;
using IWshRuntimeLibrary;
using ModernWpf.Controls;

namespace System_Init_Toolbox___Setup
{
    public partial class MainWindow : Window
    {
        public MainWindow()
        {
            InitializeComponent();
        }
        public static void Delay(int mm)
        {
            DateTime current = DateTime.Now;
            while (current.AddMilliseconds(mm) > DateTime.Now)
            {
                System.Windows.Forms.Application.DoEvents();
            }
            return;
        }
        public static bool UnZip(string fileToUnZip, string zipedFolder, string password)//解压函数
        {
            bool result = true;
            FileStream fs = null;
            ZipInputStream zipStream = null;
            ZipEntry ent = null;
            string fileName;

            if (!System.IO.File.Exists(fileToUnZip))
                return false;

            if (!Directory.Exists(zipedFolder))
                Directory.CreateDirectory(zipedFolder);

            try
            {
                zipStream = new ZipInputStream(System.IO.File.OpenRead(fileToUnZip));
                if (!string.IsNullOrEmpty(password)) zipStream.Password = password;
                while ((ent = zipStream.GetNextEntry()) != null)
                {
                    if (!string.IsNullOrEmpty(ent.Name))
                    {
                        fileName = Path.Combine(zipedFolder, ent.Name);
                        fileName = fileName.Replace('/', '\\');//change by Mr.HopeGi

                        int index = ent.Name.LastIndexOf('/');
                        if (index != -1 || fileName.EndsWith("\\"))
                        {
                            string tmpDir = (index != -1 ? fileName.Substring(0, fileName.LastIndexOf('\\')) : fileName) + "\\";
                            if (!Directory.Exists(tmpDir))
                            {
                                Directory.CreateDirectory(tmpDir);
                            }
                            if (tmpDir == fileName)
                            {
                                continue;
                            }
                        }

                        fs = System.IO.File.Create(fileName);
                        int size = 2048;
                        byte[] data = new byte[size];
                        while (true)
                        {
                            size = zipStream.Read(data, 0, data.Length);
                            if (size > 0)
                                fs.Write(data, 0, data.Length);
                            else
                                break;
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                MessageBox.Show("Error", "安装失败！\n解压程序文件包时出错，错误信息：\n" + ex,MessageBoxButton.OK, MessageBoxImage.Error);
                result = false;
            }
            finally
            {
                if (fs != null)
                {
                    fs.Close();
                    fs.Dispose();
                }
                if (zipStream != null)
                {
                    zipStream.Close();
                    zipStream.Dispose();
                }
                if (ent != null)
                {
                    ent = null;
                }
                GC.Collect();
                GC.Collect(1);
            }
            return result;
        }
        public static void CreateShortcut(string directory, string shortcutName, string targetPath,
            string description = null, string iconLocation = null)
        {
            if (!System.IO.Directory.Exists(directory))
            {
                System.IO.Directory.CreateDirectory(directory);
            }

            string shortcutPath = Path.Combine(directory, string.Format("{0}.lnk", shortcutName));
            WshShell shell = new WshShell();
            IWshShortcut shortcut = (IWshShortcut)shell.CreateShortcut(shortcutPath);//创建快捷方式对象
            shortcut.TargetPath = targetPath;//指定目标路径
            shortcut.WorkingDirectory = Path.GetDirectoryName(targetPath);//设置起始位置
            shortcut.WindowStyle = 1;//设置运行方式，默认为常规窗口
            shortcut.Description = description;//设置备注
            shortcut.IconLocation = string.IsNullOrWhiteSpace(iconLocation) ? targetPath : iconLocation;//设置图标路径
            shortcut.Save();//保存快捷方式
        }
        private void Button_Click(object sender, RoutedEventArgs e)
        {
            string path = "C:/Program Files/Stargazing Studio/System Init Toolbox";
            //如果已经存在默认路径了
            if (Directory.Exists(path))
            {
                //那就什么都不干
            }
            else
            {
                //不然就创建
                Process nf_Process = new Process();
                ProcessStartInfo startInfo = new ProcessStartInfo("./SetupBATFiles/mkdir_mr.bat");
                startInfo.CreateNoWindow = true;//无窗口
                nf_Process.StartInfo = startInfo;
                nf_Process.Start();
                nf_Process.WaitForExit();
            }
            //复制文件
            status_label.Content = "复制文件包中...";
            pb.Value = 25;
            Delay(1000);//给予label和pb.value切换的反应时间
            string sourcePath = "win10-x64.zip";//自解压后程序压缩包名
            string targetPath = path;//目标位置 直接默认目录
            bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
            System.IO.File.Copy(sourcePath, targetPath, isrewrite);//复制~
            //解压文件
            status_label.Content = "解压文件包中...";
            pb.Value = 75;
            Delay(1000);
            if (UnZip(path + "/win10-x64.zip", path, null))//如果解压成功
            {
                System.IO.File.Delete(path + "/win10-x64.zip");//删除残留文件包
                CreateShortcut("%USERPROFILE%/Desktop", "System Init Toolbox", path + "/System Init Toolbox.exe", "【System Init Toolbox · 系统初始化工具箱】一个能让你在安装系统后快速安装运行库等必备程序的软件",path+"/System Init Toolbox.exe");//创建桌面快捷方式
                CreateShortcut(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs", "System Init Toolbox", path + "/System Init Toolbox.exe", "【System Init Toolbox · 系统初始化工具箱】一个能让你在安装系统后快速安装运行库等必备程序的软件", path + "/System Init Toolbox.exe");//创建任务栏快捷方式
            }
            else
            {
                MessageBox.Show("Error", "安装失败",MessageBoxButton.OK,MessageBoxImage.Error);
                Environment.Exit(0);
            }
        }

        private async void Button_Click_1(object sender, RoutedEventArgs e)
        {
            string path = string.Empty;
            System.Windows.Forms.FolderBrowserDialog scpath = new System.Windows.Forms.FolderBrowserDialog();
            if (scpath.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                path = scpath.SelectedPath;
                //由于是自选目录所以几乎不可能存在没这路径的情况，自然没必要进行判断了。
                //复制文件
                status_label.Content = "复制文件包中...";
                pb.Value = 25;
                Delay(1000);//给予label和pb.value切换的反应时间
                string sourcePath = "win10-x64.zip";//自解压后程序压缩包名
                string targetPath = path;//目标位置 直接默认目录
                bool isrewrite = true; // true=覆盖已存在的同名文件,false则反之
                System.IO.File.Copy(sourcePath, targetPath, isrewrite);//复制~
                                                                       //解压文件
                status_label.Content = "解压文件包中...";
                pb.Value = 75;
                Delay(1000);
                if (UnZip(path + "/win10-x64.zip", path, null))//如果解压成功
                {
                    System.IO.File.Delete(path + "/win10-x64.zip");//删除残留文件包
                    CreateShortcut("%USERPROFILE%/Desktop", "System Init Toolbox", path + "/System Init Toolbox.exe", "【System Init Toolbox · 系统初始化工具箱】一个能让你在安装系统后快速安装运行库等必备程序的软件", path + "/System Init Toolbox.exe");//创建桌面快捷方式
                    CreateShortcut(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs", "System Init Toolbox", path + "/System Init Toolbox.exe", "【System Init Toolbox · 系统初始化工具箱】一个能让你在安装系统后快速安装运行库等必备程序的软件", path + "/System Init Toolbox.exe");//创建任务栏快捷方式
                }
                else
                {
                    MessageBox.Show("Error", "安装失败", MessageBoxButton.OK, MessageBoxImage.Error);
                    Environment.Exit(0);
                }
            }
            else
            {
                ContentDialog noselectDialog = new ContentDialog
                {
                    Title = "未选择目录",
                    Content = "请选选择一个用于存放程序文件的目录再继续安装哦~若您不想选择，直接以默认路径安装也是可以的哦！",
                    CloseButtonText = "Ok"
                };
                ContentDialogResult result = await noselectDialog.ShowAsync();
            }
        }
    }
}
