using System.IO.Abstractions;
using DryIoc;
using gOldCleaner.ApplicationServices;
using gOldCleaner.InfrastructureServices;
using NLog;

namespace gOldCleaner
{
    public static class CompositionRoot
    {
        public static Container Container { get; set; } = new Container();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        static CompositionRoot()
        {
            InitContainer();
        }

        private static void InitContainer()
        {
            Container.Register<IApplicationService, ApplicationService>();
            Container.Register<IStorageService, FsService>();

            Container.Register<IFileSystem, FileSystem>(Reuse.Singleton);
            Container.Use<ILogger>(Log);

            //Container.UseInstance(InitMappings());
        }
    }
}

#region Snippets

/*

        Container.Register<ITransferService, SshTransferService>(Made.Of(() =>
                    new SshTransferService(
                        Arg.Index<SftpClient>(0)
                    ),
                f0 => new SftpClient(Default.SshHost, Default.SshUser, Default.SshPwdEnc.GDecrypt())
            ), Reuse.Singleton);


                    private static IMapper InitMappings()
                    {
                        var config = new MapperConfiguration(cfg =>
                        {
                            cfg.AddProfile(new MappingProfileRoot());
                        });

                        return config.CreateMapper();
                    }
        */

#endregion
