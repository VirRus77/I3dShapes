using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using I3dShapes.Tools;
using I3dShapes.Tools.Extensions;
using Microsoft.Extensions.Logging;

namespace I3dShapes.Container
{
    /// <summary>
    /// Class by work from Crypto container.
    /// </summary>
    public class FileContainer
    {
        private readonly ILogger _logger;
        private readonly IDecryptor _decryptor;

        /// <summary>
        /// Constructor <see cref="FileContainer"/>.
        /// </summary>
        /// <param name="filePath"></param>
        /// <param name="logger"></param>
        /// <exception cref="FileNotFoundException"></exception>
        public FileContainer(string filePath, ILogger logger = null)
        {
            FilePath = filePath;
            _logger = logger;


            if (!File.Exists(FilePath))
            {
                _logger?.LogCritical("File not found: {filePath}.", filePath);
                throw new FileNotFoundException("File not found.", filePath);
            }

            Initialize();

            _decryptor = new Decryptor(Header.Seed);
        }

        /// <summary>
        /// File path.
        /// </summary>
        public string FilePath { get; }

        /// <summary>
        /// Header Crypto container.
        /// </summary>
        public FileHeader Header { get; private set; }

        /// <summary>
        /// Encrypted content.
        /// </summary>
        public bool IsEncrypted { get; set; }

        /// <summary>
        /// Bit-endian file.
        /// </summary>
        public Endian Endian { get; private set; }

        /// <summary>
        /// Read all <see cref="Entity"/> in file.
        /// </summary>
        /// <returns>Collection <see cref="Entity"/></returns>
        public ICollection<Entity> GetEntities()
        {
            return ReadEntities(_decryptor, FilePath, IsEncrypted);
        }

        /// <summary>
        /// Read many <see cref="Entity"/>.
        /// </summary>
        /// <param name="entities">Collection <see cref="Entity"/>.</param>
        /// <returns></returns>
        public IEnumerable<(Entity Entity, byte[] RawData)> ReadRawData(IEnumerable<Entity> entities)
        {
            using var stream = File.OpenRead(FilePath);
            foreach (var entity in entities)
            {
                yield return (Entity: entity, RawData: ReadRawData(stream, entity));
            }
        }

        /// <summary>
        /// Read content <see cref="Entity"/>.
        /// </summary>
        /// <param name="entity"><inheritdoc cref="Entity"/></param>
        /// <returns>Decrypt content <see cref="Entity"/>.</returns>
        public byte[] ReadRawData(Entity entity)
        {
            using var stream = File.OpenRead(FilePath);
            return ReadRawData(stream, entity);
        }

        private void Initialize()
        {
            Header = ReadHeader(FilePath);
            _logger?.LogDebug("File seed: {fileSeed}", Header.Seed);
            _logger?.LogDebug("File version: {version}", Header.Version);

            if (Header.Version < 2 || Header.Version > 5)
            {
                _logger?.LogCritical("Unsupported version: {version}", Header.Version);
                throw new NotSupportedException("Unsupported version");
            }

            Endian = GetEndian(Header.Version);
            IsEncrypted = GetIsEncrypted(Header);
        }

        private byte[] ReadRawData(Stream stream, Entity entity)
        {
            stream.Seek(entity.OffsetRawBlock, SeekOrigin.Begin);
            var buffer = stream.Read((int)entity.Size);

            if (IsEncrypted)
            {
                _decryptor.Decrypt(buffer, entity.DecryptIndexBlock);
            }

            return buffer;
        }

        /// <summary>
        /// Read and decrypt <see cref="UInt32"/>.
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/>.</param>
        /// <param name="decryptor"><inheritdoc cref="IDecryptor"/></param>
        /// <param name="cryptBlockIndex"></param>
        /// <param name="endian"></param>
        /// <returns></returns>
        internal static uint ReadDecryptUInt32(in Stream stream, in IDecryptor decryptor, in ulong cryptBlockIndex, in Endian endian)
        {
            var nextCryptBlockIndex = 0ul;
            return ReadDecryptUInt32(stream, decryptor, cryptBlockIndex, ref nextCryptBlockIndex, endian);
        }

        internal static uint ReadDecryptUInt32(
            in Stream stream,
            in IDecryptor decryptor,
            in ulong cryptBlockIndex,
            ref ulong nextCryptBlockIndex,
            in Endian endian
        )
        {
            var buffer = stream.Read(sizeof(int));
            decryptor.Decrypt(buffer, cryptBlockIndex, ref nextCryptBlockIndex);
            if (endian == Endian.Big)
            {
                Array.Reverse(buffer);
            }

            return BitConverter.ToUInt32(buffer, 0);
        }

        /// <summary>
        /// Read <inheritdoc cref="FileHeader"/> by file name
        /// </summary>
        /// <param name="fileName"></param>
        /// <returns></returns>
        private static FileHeader ReadHeader(string fileName)
        {
            using (var stream = File.OpenRead(fileName))
            {
                return ReadHeader(stream);
            }
        }

        /// <summary>
        /// Read <inheritdoc cref="FileHeader"/> by <inheritdoc cref="Stream"/>
        /// </summary>
        /// <param name="stream"><inheritdoc cref="Stream"/></param>
        /// <returns><inheritdoc cref="FileHeader"/></returns>
        private static FileHeader ReadHeader(Stream stream)
        {
            var readHeader = FileHeader.Read(stream);
            return readHeader;
        }

        /// <summary>
        /// Read all <see cref="Entity"/> in file.
        /// </summary>
        /// <param name="decryptor"></param>
        /// <param name="fileName"></param>
        /// <param name="isEncrypted">Encryted file.</param>
        /// <returns></returns>
        private static ICollection<Entity> ReadEntities(IDecryptor decryptor, in string fileName, bool isEncrypted = true)
        {
            using var stream = File.OpenRead(fileName);
            var header = ReadHeader(stream);
            var endian = GetEndian(header.Version);

            var cryptBlockIndex = 0ul;

            var countEntities = isEncrypted
                ? ReadDecryptUInt32(stream, decryptor, cryptBlockIndex, ref cryptBlockIndex, endian)
                : stream.ReadUInt32(endian);

            return Enumerable
                .Range(0, (int)countEntities)
                .Select(v => Entity.Read(stream, decryptor, ref cryptBlockIndex, endian, isEncrypted))
                .ToArray();
        }

        internal static ulong RoundUp(in ulong value, in ulong toNearest)
        {
            return (value + toNearest - 1) / toNearest;
        }

        private static Endian GetEndian(in short version)
        {
            return version >= 4 ? Endian.Little : Endian.Big;
        }

        private bool GetIsEncrypted(FileHeader header)
        {
            return header.Version != 2 && header.Seed != 0;
        }
    }
}
