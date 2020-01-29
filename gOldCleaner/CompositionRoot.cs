using System.IO.Abstractions;
using DryIoc;
using gOldCleaner.Domain;
using gOldCleaner.InfrastructureServices;
using NLog;

namespace gOldCleaner
{
    public static class CompositionRoot
    {
        public static Container Container { get; set; } = new Container();
        private static readonly Logger _log = LogManager.GetCurrentClassLogger();

        static CompositionRoot()
        {
            InitContainer();
        }

        private static void InitContainer()
        {
            Container.Register<IStorageService, StorageService>(Reuse.Singleton);
            Container.Register<IItemsRoot, ItemsRoot>(Reuse.Singleton);
            Container.Register<IFileSystem, FileSystem>(Reuse.Singleton);
            
            Container.Use<ILogger>(_log);

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
