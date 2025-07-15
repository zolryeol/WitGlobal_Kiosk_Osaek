using UnityEditor;
using UnityEngine;
using Google.Apis.Sheets.v4.Data;
using System.IO;
using System.Threading.Tasks;

/// <summary>
///  현재 저장이 필요없기때문에 SO는 필요없음 추후 필요하다면 고쳐서 사용할 것
/// </summary>
//public class ShopInfoSOGenerator
//{
//    private const string AssetPath = "Assets/Resources/ShopInfoDataSO.asset";

//    [MenuItem("Tools/ShopInfo/Generate ShopInfoDataSO")]
//    public static async void GenerateShopInfoDataSO()
//    {
//        try
//        {
//            // 1. GoogleSheetReader로 시트 데이터 비동기 읽기
//            ValueRange sheet = await GoogleSheetReader.ReadSheetAsync();

//            if (sheet == null)
//            {
//                Debug.LogError("GoogleSheetReader.ReadSheetAsync() 결과가 null입니다.");
//                return;
//            }

//            // 2. ShopSheetParser로 파싱
//            var shopList = ShopSheetParser.Parse(sheet);

//            if (shopList == null)
//            {
//                Debug.LogError("ShopSheetParser.Parse 결과가 null입니다.");
//                return;
//            }

//            // 3. ScriptableObject 생성 또는 갱신
//            ShopInfoDataSO so = AssetDatabase.LoadAssetAtPath<ShopInfoDataSO>(AssetPath);
//            if (so == null)
//            {
//                so = ScriptableObject.CreateInstance<ShopInfoDataSO>();
//                AssetDatabase.CreateAsset(so, AssetPath);
//            }

//            // 값이 변경된 경우에만 Dirty 처리
//            if (so.shopList == null || !so.shopList.Equals(shopList))
//            {
//                so.shopList = shopList;
//                EditorUtility.SetDirty(so);
//                AssetDatabase.SaveAssets();
//            }

//            Debug.Log($"ShopInfoDataSO 자동 생성/갱신 완료! ({shopList.Count}개)");
//        }
//        catch (System.Exception ex)
//        {
//            Debug.LogError($"ShopInfoDataSO 생성 중 오류 발생: {ex.Message}\n{ex.StackTrace}");
//        }
//    }
//}