import subprocess, json, os, base64, sys

OWNER = "wuxiaojun283264941-star"
REPO = "OccupationalHealth-Win-CSharp"
BASE_DIR = r"F:\ZHIYEWEISHENGDAIMA\WORK\CSHARP"

def gh_api(method, endpoint, data=None):
    cmd = ["gh", "api", f"/repos/{OWNER}/{REPO}{endpoint}", "--method", method, "--header", "Accept: application/vnd.github.v3+json"]
    if data is not None:
        cmd.extend(["--input", "-"])
        result = subprocess.run(cmd, input=json.dumps(data).encode(), capture_output=True, timeout=30)
    else:
        result = subprocess.run(cmd, capture_output=True, timeout=30)
    
    if result.returncode != 0:
        print(f"  API ERROR ({endpoint}): {result.stderr.decode()[:200]}")
        return None
    try:
        return json.loads(result.stdout)
    except:
        return None

def main():
    print(f"=== Pushing to GitHub: {OWNER}/{REPO} ===\n")
    
    # Gather files
    ignore = {'.git', '__pycache__', 'publish', 'bin', 'obj', '.vs', 'node_modules'}
    files = []
    for root, dirs, filenames in os.walk(BASE_DIR):
        dirs[:] = [d for d in dirs if d not in ignore and not d.startswith('.')]
        for f in filenames:
            fp = os.path.join(root, f)
            rp = os.path.relpath(fp, BASE_DIR).replace('\\', '/')
            if any(rp.startswith(p) for p in ignore):
                continue
            if f.endswith(('.pyc', '.db')):
                continue
            files.append((rp, fp))
    
    print(f"Total: {len(files)} files\n")
    
    # Create blobs
    tree_items = []
    blob_cache = {}
    
    for rp, fp in files:
        # Read file
        with open(fp, 'rb') as f:
            raw = f.read()
        
        # Try as text first
        try:
            text = raw.decode('utf-8')
            blob_data = gh_api("POST", "/git/blobs", {"content": text, "encoding": "utf-8"})
        except:
            # Binary file
            b64 = base64.b64encode(raw).decode()
            blob_data = gh_api("POST", "/git/blobs", {"content": b64, "encoding": "base64"})
        
        if blob_data and 'sha' in blob_data:
            sha = blob_data['sha']
            blob_cache[rp] = sha
            tree_items.append({"path": rp, "mode": "100644", "type": "blob", "sha": sha})
            print(f"  ✓ {rp}")
        else:
            print(f"  ✗ {rp}")
    
    if not tree_items:
        print("\nNo blobs created!")
        return
    
    print(f"\n[{len(tree_items)} blobs created] Creating tree...")
    
    # Create tree
    tree_data = gh_api("POST", "/git/trees", {"tree": tree_items})
    if not tree_data or 'sha' not in tree_data:
        print("Failed to create tree!")
        return
    
    print(f"Tree: {tree_data['sha']}")
    
    # Create commit
    commit_data = gh_api("POST", "/git/commits", {
        "message": "Initial commit: ASP.NET Core 8 职业健康体检管理平台",
        "tree": tree_data['sha'],
        "parents": []
    })
    if not commit_data or 'sha' not in commit_data:
        print("Failed to create commit!")
        return
    
    print(f"Commit: {commit_data['sha']}")
    
    # Create or update branch ref
    ref_data = gh_api("POST", "/git/refs", {
        "ref": "refs/heads/main",
        "sha": commit_data['sha']
    })
    
    if ref_data:
        print(f"\n=== PUSH COMPLETE ===")
        print(f"https://github.com/{OWNER}/{REPO}")
    else:
        print("\nUpdating existing ref instead...")
        ref_data = gh_api("PATCH", "/git/refs/heads/main", {
            "sha": commit_data['sha'],
            "force": True
        })
        if ref_data:
            print(f"\n=== PUSH COMPLETE ===")
            print(f"https://github.com/{OWNER}/{REPO}")
        else:
            print("\nFailed!")

if __name__ == "__main__":
    main()
