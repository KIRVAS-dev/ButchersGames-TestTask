// CLAUDE.md is loaded once and drifts out of focus in long sessions; the
// question format and the task lifecycle were the rules most often skipped.
process.stdout.write(JSON.stringify({
  hookSpecificOutput: {
    hookEventName: "UserPromptSubmit",
    additionalContext:
      "CLAUDE.md: questions — numbered text, then AskUserQuestion (>4 options → extra question); Task lifecycle stage → offer the next step; unrelated request → offer handoff.",
  },
}));
