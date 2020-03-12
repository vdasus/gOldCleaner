using DryIoc;
using gOldCleaner.InfrastructureServices;
using NLog;
using System.IO.Abstractions;
using CSharpFunctionalExtensions;
using gOldCleaner.DomainServices;

namespace gOldCleaner
{
    public static class CompositionRoot
    {
        public static Container Container { get; } = new Container();
        private static readonly Logger Log = LogManager.GetCurrentClassLogger();

        static CompositionRoot()
        {
            Result.ErrorMessagesSeparator = "\n";
            InitContainer();
        }

        public static void BuildStorage(bool isFakeFs)
        {
            if (isFakeFs)
                Container.Register<IStorageService, FakeStorageService>(Reuse.Singleton);
            else
                Container.Register<IStorageService, StorageService>(Reuse.Singleton);
        }

        private static void InitContainer()
        {
            Container.Register<IFileSystem, FileSystem>(Reuse.Singleton);
            
            Container.Register<IFolderItemService, FolderItemService>(Reuse.Singleton);
            Container.Register<IInformerService, InformerService>(Reuse.Singleton);
            
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
