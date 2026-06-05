"""生成职业健康体检管理平台桌面图标 (256x256 + 多尺寸 ICO)"""
from PIL import Image, ImageDraw, ImageFont
import struct, io

SIZE = 256

def create_icon_png():
    img = Image.new('RGBA', (SIZE, SIZE), (0, 0, 0, 0))
    draw = ImageDraw.Draw(img)

    cx, cy = SIZE // 2, SIZE // 2
    r = SIZE // 2 - 8

    # ---- 1. 盾牌外轮廓 (深蓝渐变) ----
    shield_points = [
        (cx, cy - r),                          # 顶部
        (cx + r, cy - r // 3),                  # 右上
        (cx + r, cy + r // 3),                  # 右中
        (cx, cy + r),                           # 底部
        (cx - r, cy + r // 3),                  # 左中
        (cx - r, cy - r // 3),                  # 左上
    ]
    
    # 外圈阴影
    offset = 3
    shadow = [(x + offset, y + offset) for x, y in shield_points]
    draw.polygon(shadow, fill=(0, 0, 0, 40))
    
    # 主盾牌 - 渐变效果用多层
    for i in range(5):
        ratio = 1 - i * 0.04
        shrink = int(r * 0.05 * i)
        pts = []
        for x, y in shield_points:
            dx, dy = x - cx, y - cy
            pts.append((cx + dx * ratio, cy + dy * ratio + shrink))
        b = 130 - i * 12
        draw.polygon(pts, fill=(25, 50, b, 230))

    # 盾牌高光边框
    highlight = []
    for x, y in shield_points:
        dx, dy = x - cx, y - cy
        highlight.append((cx + dx * 0.96, cy + dy * 0.96 - 2))
    draw.polygon(highlight, outline=(100, 160, 255, 180), width=2)

    # ---- 2. 内部圆形底色 (白色半透明) ----
    inner_r = int(r * 0.62)
    draw.ellipse([cx - inner_r, cy - inner_r + 4, cx + inner_r, cy + inner_r + 4], fill=(255, 255, 255, 220))

    # ---- 3. 医疗十字 (红色) ----
    cross_w = int(inner_r * 0.85)
    cross_h = int(inner_r * 0.38)
    arm_w = int(cross_h * 0.55)
    
    # 垂直条
    draw.rounded_rectangle(
        [cx - arm_w, cy - cross_w // 2, cx + arm_w, cy + cross_w // 2],
        radius=6, fill=(220, 30, 50, 230)
    )
    # 水平条
    draw.rounded_rectangle(
        [cx - cross_w // 2, cy - arm_w, cx + cross_w // 2, cy + arm_w],
        radius=6, fill=(220, 30, 50, 230)
    )

    # ---- 4. 十字底部的小装饰 ----
    # 两个齿轮半圆
    gear_r = int(inner_r * 0.14)
    for angle in [30, -30]:
        import math
        gx = cx + int(inner_r * 0.58 * math.cos(math.radians(angle)))
        gy = cy + int(inner_r * 0.58 * math.sin(math.radians(angle))) + 4
        # 外圈
        draw.ellipse([gx - gear_r, gy - gear_r, gx + gear_r, gy + gear_r], 
                     outline=(60, 120, 200, 180), width=2)
        # 内点
        dot_r = int(gear_r * 0.35)
        draw.ellipse([gx - dot_r, gy - dot_r, gx + dot_r, gy + dot_r],
                     fill=(60, 120, 200, 180))

    # ---- 5. 底部丝带 ----
    ribbon_y = cy + int(r * 0.65)
    ribbon_h = int(r * 0.15)
    ribbon_w = int(r * 0.9)
    
    ribbon_points = [
        (cx - ribbon_w // 2, ribbon_y),
        (cx + ribbon_w // 2, ribbon_y),
        (cx + ribbon_w // 2 - 8, ribbon_y + ribbon_h),
        (cx - ribbon_w // 2 + 8, ribbon_y + ribbon_h),
    ]
    draw.polygon(ribbon_points, fill=(25, 80, 160, 230))

    # ---- 6. 文字 "卫生" (简化: 两个小方块代表) ----
    # 在丝带上放两个白色小方块代表文字
    text_block_w = int(ribbon_w * 0.12)
    text_block_h = int(ribbon_h * 0.45)
    text_y = ribbon_y + (ribbon_h - text_block_h) // 2
    
    for tx_offset in [-int(ribbon_w * 0.15), int(ribbon_w * 0.15)]:
        tx = cx + tx_offset
        draw.rounded_rectangle(
            [tx - text_block_w // 2, text_y, tx + text_block_w // 2, text_y + text_block_h],
            radius=2, fill=(180, 220, 255, 200)
        )

    return img

def save_ico(png_img, filepath):
    """将 PNG 保存为多尺寸 ICO 文件"""
    # ICO 支持的尺寸
    sizes = [256, 128, 64, 48, 32, 24, 16]
    
    # 准备所有尺寸的 PNG 数据
    png_datas = {}
    best_size = 256
    for s in sorted(sizes, reverse=True):
        if s <= best_size:
            resized = png_img.resize((s, s), Image.LANCZOS)
            buf = io.BytesIO()
            resized.save(buf, format='PNG')
            png_datas[s] = buf.getvalue()

    with open(filepath, 'wb') as f:
        # ICO Header
        f.write(struct.pack('<HHH', 0, 1, len(png_datas)))
        
        # 计算偏移
        offset = 6 + 16 * len(png_datas)
        dir_entries = []
        
        for s in sorted(png_datas.keys(), reverse=True):
            data = png_datas[s]
            bmp_size = len(data)
            # 规范: ICO 目录条目
            # 对于 PNG 格式，需要写入 40-byte BITMAPINFOHEADER + 像素数据
            # 但 Windows 也支持直接在 ICO 中嵌入 PNG 数据
            
            # 生成 BMP 格式数据嵌入 ICO
            resized = png_img.resize((s, s), Image.LANCZOS)
            
            # 写入 BMP 格式数据到内存
            bmp_buf = io.BytesIO()
            # BMP 文件头
            bmp_data = bytearray()
            
            # BITMAPINFOHEADER (40 bytes)
            bmp_header = struct.pack('<IiiHHIIiiII',
                40,              # biSize
                s,               # biWidth
                s * 2,           # biHeight (double for ICO)
                1,               # biPlanes
                32,              # biBitCount
                0,               # biCompression (BI_RGB)
                0,               # biSizeImage
                0, 0,            # biXPelsPerMeter, biYPelsPerMeter
                0, 0             # biClrUsed, biClrImportant
            )
            bmp_data.extend(bmp_header)
            
            # 像素数据 (BGRA, 从底部向上, 每行对齐到4字节)
            pixels = resized.load()
            for y in range(s - 1, -1, -1):
                row = bytearray()
                for x in range(s):
                    r, g, b, a = pixels[x, y]
                    row.extend([b, g, r, a])
                # 行对齐到4字节
                pad = (4 - len(row) % 4) % 4
                row.extend([0] * pad)
                bmp_data.extend(row)
            
            # 在 AND 掩码位置写入 0 (对 32bpp 来说, 不需要 AND 掩码)
            # 对 32bpp 图像, biHeight 已经是 s*2, 包含了 AND 掩码区域
            # 但我们写入的是完整像素数据, AND 掩码部分应该是 0
            and_size = ((s + 31) // 32) * 4
            bmp_data.extend([0] * and_size)
            
            # ICO directory entry: width/height 0 means 256
            w = 0 if s >= 256 else s
            f.write(struct.pack('<BBBBHHII',
                w,              # bWidth (0 = 256)
                w,              # bHeight (0 = 256)
                0,              # bColorCount
                0,              # bReserved
                1,              # wPlanes
                32,             # wBitCount
                len(bmp_data),  # dwBytesInRes
                offset          # dwImageOffset
            ))
            dir_entries.append((offset, bmp_data))
            offset += len(bmp_data)
        
        # 写入实际的图像数据
        for offset, data in dir_entries:
            f.write(data)

if __name__ == '__main__':
    img = create_icon_png()
    output_path = 'F:\\ZHIYEWEISHENGDAIMA\\WORK\\CSHARP\\src\\OccupationalHealth.Api\\wwwroot\\favicon.ico'
    save_ico(img, output_path)
    print(f'Icon saved to: {output_path}')
    
    # 同时保存 PNG 预览
    preview_path = 'F:\\ZHIYEWEISHENGDAIMA\\WORK\\CSHARP\\icon_preview.png'
    img.save(preview_path, 'PNG')
    print(f'Preview saved to: {preview_path}')
