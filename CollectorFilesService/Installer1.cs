using System.ComponentModel;
using System.Configuration.Install;
using System.ServiceProcess;

namespace CollectorFilesService
{
    [RunInstaller(true)]
    public partial class Installer1 : Installer
    {
        ServiceInstaller serviceInstaller;
        ServiceProcessInstaller processInstaller;

        public Installer1()
        {
            InitializeComponent();

            serviceInstaller = new ServiceInstaller();
            processInstaller = new ServiceProcessInstaller();

            //processInstaller.Account = ServiceAccount.User;
            processInstaller.Account = ServiceAccount.LocalSystem;
            serviceInstaller.StartType = ServiceStartMode.Manual;
            serviceInstaller.Description = "ВАЖНО! Для запуска введите в строку 'Параметры запуска'" +
                " путь к папке с настройками и лог-файлом";
            serviceInstaller.ServiceName = "CollectorFilesService";
            Installers.Add(processInstaller);
            Installers.Add(serviceInstaller);
        }
    }
}
