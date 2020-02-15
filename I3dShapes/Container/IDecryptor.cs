namespace I3dShapes.Container
{
    /// <summary>
    /// Decryptor.
    /// </summary>
    public interface IDecryptor
    {
        /// <summary>
        /// Decrypt buffer by decrypt block index
        /// </summary>
        /// <param name="buffer">Decrypt block</param>
        /// <param name="blockIndex">Decrypt block index by key</param>
        void DecryptBlocks(uint[] buffer, ulong blockIndex);

        /// <summary>
        /// Decrypt buffer by decrypt block index
        /// </summary>
        /// <param name="buffer">Decrypt block</param>
        /// <param name="blockIndex">Decrypt block index by key</param>
        void Decrypt(byte[] buffer, ulong blockIndex);

        /// <summary>
        /// Decrypt block
        /// </summary>
        /// <param name="buffer">Decrypt block</param>
        /// <param name="blockIndex">Decrypt block index</param>
        /// <param name="nextBlockIndex">Next decrypt block index</param>
        void Decrypt(byte[] buffer, ulong blockIndex, ref ulong nextBlockIndex);
    }
}
