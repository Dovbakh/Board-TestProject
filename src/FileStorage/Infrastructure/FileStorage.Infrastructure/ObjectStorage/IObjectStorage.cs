using Minio.DataModel;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace FileStorage.Infrastructure.ObjectStorage
{
    public interface IObjectStorage
    {
        /// <summary>
        /// Получить обьект по имени и корзине.
        /// </summary>
        /// <param name="objectName">Имя обьекта.</param>
        /// <param name="bucketName">Имя корзины.</param>
        /// <returns>Массив байтов с содержимым обьекта.</returns>
        Task<byte[]> GetData(string objectName, string bucketName, CancellationToken cancellation);

        /// <summary>
        /// Получить обьект по имени и корзине.
        /// </summary>
        /// <param name="objectName">Имя обьекта.</param>
        /// <param name="bucketName">Имя корзины.</param>
        /// <returns>Массив байтов с содержимым обьекта.</returns>
        Task<ObjectStat> GetInfo(string objectName, string bucketName, CancellationToken cancellation);

        /// <summary>
        /// Добавить обьект с указанным именем, корзиной и типом контента.
        /// </summary>
        /// <param name="objectName">Имя обьекта.</param>
        /// <param name="bucketName">Имя корзины.</param>
        /// <param name="contentType">Тип контента.</param>
        /// <param name="bytes">Массив байтов с содержимым обьекта.</param>
        Task Upload(string objectName, string bucketName, string contentType, byte[] bytes, CancellationToken cancellation);

        /// <summary>
        /// Удалить обьект с указанным именем из указанной корзины.
        /// </summary>
        /// <param name="objectName"></param>
        /// <param name="bucketName"></param>
        /// <returns></returns>
        Task Delete(string objectName, string bucketName, CancellationToken cancellation);
    }
}
