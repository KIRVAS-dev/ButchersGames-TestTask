let raw = "";
process.stdin.on("data", (chunk) => { raw += chunk; });
process.stdin.on("end", () => {
  let input;
  try {
    input = JSON.parse(raw);
  } catch {
    return;
  }

  const toolName = input.tool_name || "";
  const action = (input.tool_input || {}).action || "";

  // Path-scoped assets.md loads only when Claude reads a matching file, but
  // assets, prefabs and scene objects are created through Coplay without a
  // Read. Remind only on actions that add, move or rename something.
  const structuralActions = {
    mcp__unityMCP__manage_prefabs: ["create_from_gameobject"],
    mcp__unityMCP__manage_asset: ["create", "duplicate", "move", "rename", "import", "create_folder"],
    mcp__unityMCP__manage_gameobject: ["create", "duplicate"],
  };
  const isImport = toolName === "mcp__unityMCP__import_model" || toolName === "mcp__unityMCP__import_model_file";
  const isStructural = (structuralActions[toolName] || []).includes(action);

  if (!isImport && !isStructural) {
    return;
  }

  process.stdout.write(JSON.stringify({
    hookSpecificOutput: {
      hookEventName: "PreToolUse",
      additionalContext:
        "Creating / moving / renaming an asset, prefab or scene object: if .claude/rules/assets.md is not in context yet, read it and follow its folders, naming and prefab / scene structure.",
    },
  }));
});
