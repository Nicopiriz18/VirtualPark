// <copyright file="IRewardRepository.cs" company="PlaceholderCompany">
// Copyright (c) PlaceholderCompany. All rights reserved.
// </copyright>

using VirtualPark.Domain;

namespace IRepositories;

public interface IRewardRepository
{
    IEnumerable<Reward> GetAll();

    Reward? GetById(Guid id);

    void Add(Reward reward);

    void Update(Reward reward);
}
