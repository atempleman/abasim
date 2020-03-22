using System.Linq;
using System.Threading.Tasks;
using ABASim.api.Models;
using Microsoft.EntityFrameworkCore;
using System.Collections.Generic;
using System;

namespace ABASim.api.Data
{
    public class AdminRepository : IAdminRepository
    {
        private readonly DataContext _context;

        private static Random rng = new Random();
        
        public AdminRepository(DataContext context)
        {
            _context = context;
        }

        public async Task<bool> UpdateLeagueState(int newState)
        {
            var league = await _context.Leagues.FirstOrDefaultAsync(x => x.Id == 1);
            league.StateId = newState;

            _context.Update(league);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RemoveTeamRegistration(int teamId)
        {
            var team = await _context.Teams.FirstOrDefaultAsync(x => x.Id == teamId);
            team.UserId = 0;
             _context.Update(team);
            return await _context.SaveChangesAsync() > 0;
        }

        public async Task<bool> RunInitialDraftLottery()
        {
            var teams = await _context.Teams.ToListAsync();

            List<int> teamIds = new List<int>();
            // Now get a list of the TeamIds
            foreach (Team t in teams) 
            {
                teamIds.Add(t.Id);
            }

            // Now need to randomly sort the list
            int n = teamIds.Count;  
            while (n > 1) {  
                n--;  
                int k = rng.Next(n + 1);  
                int value = teamIds[k];  
                teamIds[k] = teamIds[n];  
                teamIds[n] = value;  
            } 

            // TeamIds is now sorted
            // Now need to go through and save the draft picks
            for (int i = 1; i < 14; i++) 
            {
                for (int j = 1; j < 31; j++) 
                {
                    InitialDraft draftPick = new InitialDraft
                    {
                        Round = i,
                        Pick = j,
                        TeamId = teamIds[j - 1],
                        PlayerId = 0
                    };
                    await _context.AddAsync(draftPick);
                }
            }

            // Now need to update the league status
            var league = await _context.Leagues.FirstOrDefaultAsync();
            league.StateId = 3;
            _context.Update(league);

            return await _context.SaveChangesAsync() > 0;
        }
    }


}