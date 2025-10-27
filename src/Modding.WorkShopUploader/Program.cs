using Steamworks;
namespace WorkShopUploader
{
    public class Program
    {
        static void Main(string[] args)
        {
            var appid = uint.TryParse(args.FirstOrDefault(), out var num) ? num : 480;
            UploadToWorkshop(appid);
        }

        static void UploadToWorkshop(uint appid)
        {
            PublishedFileId_t fileId;
            var createResult = SteamUGC.CreateItem(
                new AppId_t(appid), // 你的游戏 AppID
                EWorkshopFileType.k_EWorkshopFileTypeCommunity
            );
            var onCreateItem = CallResult<CreateItemResult_t>.Create((result, failure) =>
            {
                if (result.m_eResult == EResult.k_EResultOK)
                {
                    fileId = result.m_nPublishedFileId;
                    UpdateWorkshopItem(fileId);
                }
            });
            onCreateItem.Set(createResult);
        }

        static void UpdateWorkshopItem(PublishedFileId_t fileId)
        {
            var handle = SteamUGC.StartItemUpdate(new AppId_t(480), fileId);
            SteamUGC.SetItemTitle(handle, "我的模组");
            SteamUGC.SetItemDescription(handle, "这是一个测试上传的模组");
            SteamUGC.SetItemContent(handle, "D:\\MyMod"); // 模组所在文件夹
            SteamUGC.SetItemPreview(handle, "D:\\MyMod\\preview.png");

            var submitResult = SteamUGC.SubmitItemUpdate(handle, "首次上传");
        }

    }
}
