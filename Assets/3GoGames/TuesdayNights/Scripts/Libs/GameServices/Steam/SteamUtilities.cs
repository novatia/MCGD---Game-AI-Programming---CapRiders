#if STEAM

using UnityEngine;

using Steamworks;

public static class SteamUtilities
{
    public static Sprite GetMediumAvatar(CSteamID i_SteamId)
    {
        if (!SteamManager.initializedMain)
        {
            return null;
        }

        if (!i_SteamId.IsValid())
        {
            return null;
        }

        // Medium avatar is 64x64.

        int mediumAvatar = SteamFriends.GetMediumFriendAvatar(i_SteamId);
        Texture2D texture = BuildTexture(mediumAvatar);
        Sprite sprite = TextureToSprite(texture);

        return sprite;
    }

    public static Sprite GetSmallAvatar(CSteamID i_SteamId)
    {
        if (!SteamManager.initializedMain)
        {
            return null;
        }

        if (!i_SteamId.IsValid())
        {
            return null;
        }

        // Small avatar is 32x32.

        int smallAvatar = SteamFriends.GetSmallFriendAvatar(i_SteamId);
        Texture2D texture = BuildTexture(smallAvatar);
        Sprite sprite = TextureToSprite(texture);

        return sprite;
    }

    public static Texture2D BuildTexture(int i_Image)
    {
        if (!SteamManager.initializedMain)
        {
            return null;
        }

        uint imageWidth;
        uint imageHeight;
        bool ret = SteamUtils.GetImageSize(i_Image, out imageWidth, out imageHeight);

        if (ret && imageWidth > 0 && imageHeight > 0)
        {
            byte[] image = new byte[imageWidth * imageHeight * 4];

            ret = SteamUtils.GetImageRGBA(i_Image, image, (int)(imageWidth * imageHeight * 4));

            Texture2D avatar = new Texture2D((int)imageWidth, (int)imageHeight, TextureFormat.RGBA32, false, true);
            avatar.LoadRawTextureData(image); // The image is upside down! "@ares_p: in Unity all texture data starts from "bottom" (OpenGL convention)"
            avatar.Apply();

            return avatar;
        }

        return null;
    }

    private static Sprite TextureToSprite(Texture2D i_Texture)
    {
        if (i_Texture == null)
        {
            return null;
        }

        Sprite sprite = Sprite.Create(i_Texture, new Rect(0f, 0f, i_Texture.width, i_Texture.height), new Vector2(0.5f, 0.5f));
        return sprite;
    }
}

#endif // STEAM