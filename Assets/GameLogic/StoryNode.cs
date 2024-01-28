using System.Collections.Generic;

public class StoryNode
{
    public string id;
    public List<BavisLine> introLines;
    public string userPrompt;
    // int timeLimit; // in ms
    public Dictionary<string, BavisResponse> responses;
    public StoryNode(string id, List<BavisLine> introLines, string userPrompt, Dictionary<string, BavisResponse> responses)
    {
        this.id = id;
        this.introLines = introLines;
        this.userPrompt = userPrompt;
        this.responses = responses;
    }
}

public class BavisResponse
{
    public string nextNodeId;
    public List<BavisLine> lines;
    public BavisResponse(string nextNodeId, List<BavisLine> lines)
    {
        this.nextNodeId = nextNodeId;
        this.lines = lines;
    }
}

public struct BavisLine
{
    public string line;
    public BavisLine(string line)
    {
        this.line = line;
    }
}