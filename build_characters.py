import os
import json

# 建立角色 ID (檔名) 與中文名稱的對應表
CHARACTER_NAME_MAP = {
    "1": "雅盈",
    "2": "騰勳",
    "3": "丰韡",
    "4": "昱霖",
    "5": "睿宸",
    "6": "育田",
    "7": "郁城",
    "8": "思丞",
    "9": "正皓",
    "10": "貫維",
    "11": "喻翔",
    "12": "冠宇",
    "13": "楷杰",
    "14": "詩耘",
    "15": "劭宇",
    "16": "宥鈞",
    "17": "昱叡",
    "18": "庭光",
    "19": "翊豪",
    "20": "靖仁",
    "21": "宇昕",
    "22": "恩圻",
    "23": "岳霖",
    "24": "宥璿",
    "25": "紀晴",
    "26": "朝升",
    "27": "沐璿",
    "28": "政瑋",
    "29": "睿均",
    "30": "昱全"
}

# 基礎路徑設定
char_text_dir = os.path.join('Assets', 'Resources', 'text', 'CCard')
char_img_dir = os.path.join('Assets', 'Resources', 'image', 'CCard')
output_data = []

SUPPORTED_EXTS = ['.jpg', '.JPG', '.jpeg', '.JPEG', '.png', '.PNG']

# 新增一個自訂的排序規則函式
def custom_sort_key(filename):
    if filename.endswith('.txt'):
        base_name = os.path.splitext(filename)[0]
        # 若檔名是純數字，將其轉為整數參與排序，並賦予最高優先級 (0)
        if base_name.isdigit():
            return (0, int(base_name))
        # 若檔名包含英文或其他字元，保留字串形式，賦予次要優先級 (1)
        else:
            return (1, base_name)
    # 非 .txt 檔案排到最後
    return (2, filename)

if os.path.exists(char_text_dir):
    # 在 sorted 中加入 key 參數
    for filename in sorted(os.listdir(char_text_dir), key=custom_sort_key):
        if filename.endswith('.txt'):
            base_name = os.path.splitext(filename)[0]
            txt_path = os.path.join(char_text_dir, filename)
            
            # --- 圖片偵測邏輯 (與卡牌相同) ---
            image_rel_path = ""
            if os.path.exists(char_img_dir):
                for ext in SUPPORTED_EXTS:
                    test_img_path = os.path.join(char_img_dir, base_name + ext)
                    if os.path.exists(test_img_path):
                        image_rel_path = f"Assets/Resources/image/CCard/{base_name}{ext}"
                        break
            
            if not image_rel_path:
                image_rel_path = "https://via.placeholder.com/280x400?text=角色圖片遺失"
            
            # --- 讀取文字檔內容 ---
            with open(txt_path, 'r', encoding='utf-8-sig') as f:
                lines = [line.strip() for line in f if line.strip() != ""]
                
            # 嚴謹檢查：確保至少有 8 行
            if len(lines) >= 8:
                # 從 Table 取得中文名稱，若找不到則預設顯示「未命名_檔名」
                zh_name = CHARACTER_NAME_MAP.get(base_name, f"未命名_{base_name}")

                char_data = {
                    "id": base_name,
                    "name": zh_name,  # 將中文名稱寫入 JSON
                    "image": image_rel_path,
                    "stats": {
                        "wisdom": lines[0],
                        "stamina": lines[1],
                        "reputation": lines[2]
                    },
                    "skills": {
                        "passive": {
                            "name": lines[3],
                            "desc": lines[4]
                        },
                        "active": {
                            "name": lines[5],
                            "desc": lines[6],
                            "cooldown": lines[7]
                        }
                    }
                }
                output_data.append(char_data)
            else:
                # 報錯機制：行數不足時大聲警告
                print(f"[格式錯誤] 角色 '{base_name}' 有效行數僅 {len(lines)} 行 (需 >=8 行)，已捨棄。")

# 輸出成 characters.json
with open('characters.json', 'w', encoding='utf-8') as f:
    json.dump(output_data, f, ensure_ascii=False, indent=2)

print(f"成功生成 {len(output_data)} 筆角色資料。")