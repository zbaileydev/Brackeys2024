interface ILootContainer {
    public LootTable LootTable {get; set;}
    public bool Looted {get; set;}

    public string GetLoot();
}