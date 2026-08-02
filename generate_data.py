import os
import json

# 建立英文與中文類別的對應表
CATEGORY_MAP = {
    "attack": "攻擊",
    "multiattack": "群攻",
    "defense": "防禦",
    "medicine": "藥品",
    "strengthen": "增攻",
    "special": "特殊"
}

# 處理正負數與前綴的輔助函式
def format_stat(val_str, pos_prefix, neg_prefix):
    val_str = val_str.strip()
    try:
        val = int(val_str)
        if val < 0:
            return f"{neg_prefix}{abs(val)}"
        else:
            return f"{pos_prefix}{val}"
    except ValueError:
        if val_str.startswith('-'):
            return f"{neg_prefix}{val_str[1:]}"
        return f"{pos_prefix}{val_str}"

# 基礎路徑設定
text_base_dir = os.path.join('Assets', 'Resources', 'text', 'Tool')
image_base_dir = os.path.join('Assets', 'Resources', 'image', 'Tool')
output_data = []

# 支援的圖片附檔名列表 (大小寫都包含)
SUPPORTED_EXTS = ['.jpg', '.JPG', '.jpeg', '.JPEG', '.png', '.PNG']

if os.path.exists(text_base_dir):
    for cat_eng, cat_zh in CATEGORY_MAP.items():
        cat_dir = os.path.join(text_base_dir, cat_eng)
        img_cat_dir = os.path.join(image_base_dir, cat_eng)
        
        if os.path.exists(cat_dir):
            for filename in os.listdir(cat_dir):
                if filename.endswith('.txt'):
                    base_name = os.path.splitext(filename)[0]
                    txt_path = os.path.join(cat_dir, filename)
                    
                    # --- 自動偵測圖片真實的附檔名 ---
                    image_rel_path = "" # 預設為空
                    if os.path.exists(img_cat_dir):
                        for ext in SUPPORTED_EXTS:
                            test_img_path = os.path.join(img_cat_dir, base_name + ext)
                            if os.path.exists(test_img_path):
                                # 找到了！記錄下正確的網址路徑
                                image_rel_path = f"Assets/Resources/image/Tool/{cat_eng}/{base_name}{ext}"
                                break
                    
                    # 如果真的沒找到圖片，給一個預設的錯誤圖片路徑，避免網頁壞掉
                    if not image_rel_path:
                        image_rel_path = "https://via.placeholder.com/280x320?text=圖片遺失"
                    # --------------------------------
                    
                    # 讀取文字檔內容 (過濾掉空白行)
                    with open(txt_path, 'r', encoding='utf-8') as f:
                        lines = [line.strip() for line in f.readlines() if line.strip() != ""]
                        
                    card_data = {
                        "id": base_name,
                        "category_eng": cat_eng,
                        "category_zh": cat_zh,
                        "image": image_rel_path
                    }

                    # 根據類別解析文字檔
                    if cat_eng == "special":
                        if len(lines) >= 3:
                            card_data["name"] = lines[0]
                            card_data["description"] = lines[1]
                            card_data["effect"] = lines[2]
                            card_data["is_special"] = True
                            output_data.append(card_data)
                    else:
                        if len(lines) >= 5:
                            if cat_eng == "defense":
                                prefixes = [("智防", "耗智"), ("體防", "耗體"), ("譽防", "耗譽")]
                            elif cat_eng == "strengthen":
                                prefixes = [("減智傷", "增智傷"), ("減體傷", "增體傷"), ("減譽傷", "增譽傷")]
                            else: # attack, multiattack, medicine
                                prefixes = [("治智", "傷智"), ("治體", "傷體"), ("治譽", "傷譽")]
                            
                            stat1 = format_stat(lines[0], prefixes[0][0], prefixes[0][1])
                            stat2 = format_stat(lines[1], prefixes[1][0], prefixes[1][1])
                            stat3 = format_stat(lines[2], prefixes[2][0], prefixes[2][1])
                            
                            card_data["name"] = lines[3]
                            card_data["description"] = lines[4]
                            card_data["stats"] = [stat1, stat2, stat3]
                            card_data["is_special"] = False
                            
                            output_data.append(card_data)

# 輸出成 JSON
with open('cards.json', 'w', encoding='utf-8') as f:
    json.dump(output_data, f, ensure_ascii=False, indent=2)