using ModernWpf.Controls;
using System;
using System.Windows;

namespace System_Init_Toolbox___Setup
{
    public class DialogViewer
    {
        public async static void dialog_uninstall_view(System.Windows.Controls.Label maintext, System.Windows.Controls.Button b1, System.Windows.Controls.Button b2, System.Windows.Controls.Button b3, System.Windows.Controls.Label status_label)
        {
            ContentDialog dialog_uninstall_tips = new ContentDialog
            {
                Title = "您已安装System Init Toolbox",
                Content = "很高兴您能运行System Init Toolbox - Setup程序，但您似乎已经安装了System Init Toolbox。\n您是要卸载System Init Toolbox吗？",
                SecondaryButtonText = "卸载",
                PrimaryButtonText = "退出Setup程序",
                CloseButtonText = "继续安装"
            };
            ContentDialogResult result = await dialog_uninstall_tips.ShowAsync();
            if (result == ContentDialogResult.Secondary)
            {
                MainWindow.uninstallmode = true;
                b3.Content = "卸载";
                maintext.Content = "卸载";
                b1.IsEnabled = false;
                b2.IsEnabled = false;
                b1.Opacity = 0;
                b2.Opacity = 0;
                b3.Opacity = 100;
                b3.IsEnabled = true;
                status_label.Content = "等待卸载...";
            }
            if (result == ContentDialogResult.Primary)
            {
                Environment.Exit(0);
            }
        }
        public async static void dialog_real_uninstall(System.Windows.Controls.Label status_label, System.Windows.Controls.ProgressBar pb, System.Windows.Controls.Button b3)
        {
            ContentDialog dialog_uninstall_tips_2 = new ContentDialog
            {
                Title = "真的要卸载System Init Toolbox吗？",
                Content = "如果我们有什么做得不好的地方，希望您高抬贵手，在Github Issues上给我们一些建议，每一个建议，我们都会认真查看哒！\n希望下次，我们还会再见面哦~",
                PrimaryButtonText = "继续卸载",
                CloseButtonText = "还是算了"
            };
            ContentDialogResult result_2 = await dialog_uninstall_tips_2.ShowAsync();
            if(result_2 == ContentDialogResult.Primary)
            {
                status_label.Content = "正在删除文件...";
                pb.Value = 50;
                System.IO.Directory.Delete(System.IO.File.ReadAllText(Environment.GetFolderPath(Environment.SpecialFolder.UserProfile) + "/AppData/Local/Stargazing Studio/System Init Toolbox/install_PATH"));
                pb.Value = 85;
                status_label.Content = "正在删除快捷方式...";
                System.IO.File.Delete(@"C:\ProgramData\Microsoft\Windows\Start Menu\Programs\System Init Toolbox.lnk");
                System.IO.File.Delete(Environment.GetFolderPath(Environment.SpecialFolder.Desktop) + "/System Init Toolbox.lnk");
                pb.Value = 100;
                status_label.Content = "卸载完毕";
                MainWindow.uninstallmode = false;
                b3.Content = "关闭安装程序";
            }
        }
}
}
