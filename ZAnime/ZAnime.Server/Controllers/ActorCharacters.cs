﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zanime.Server.Data;
using Zanime.Server.Models.Main;
using Zanime.Server.Models.Main.DTO.Actor_Model;

namespace Zanime.Server.Controllers
{
    [Route("api/[controller]")]
    [ApiController]
    public class ActorCharacters : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public ActorCharacters(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpPost("{CharacterID}")]
        public async Task<ActionResult<string>> CreateActorToCharacter(int CharacterID, ActorVM model)
        {
            var character = await _context.Characters
                .Include(c => c.ActorCharacters)
                    .ThenInclude(ac => ac.Actor)
                .FirstOrDefaultAsync(c => c.ID == CharacterID);

            if (character == null)
            {
                return NotFound("No character was found");
            }

            Actor actor = new Actor
            {
                Name = model.Name,
                Age = model.Age,
                Gender = model.Gender,
                PicturePath = model.PicturePath,
                Bio = model.Bio,
                Likes = 0,
                Dislikes = 0,
            };

            await _context.Actors.AddAsync(actor);
            //This is important so we can get the ID
            await _context.SaveChangesAsync();

            ActorCharacter actorCharacter = new ActorCharacter
            {
                ActorID = actor.ID,
                CharacterID = character.ID
            };

            await _context.ActorCharacters.AddAsync(actorCharacter);

            character.ActorCharacters.Add(actorCharacter);

            await _context.SaveChangesAsync();

            return Ok($"{actor.Name} was created for {character.Name}");
        }

        [HttpPost("{ActorID}")]
        public async Task<ActionResult<string>> CreateCharacterToActor(int ActorID, ActorVM model)
        {
            var actor = await _context.Actors
                .Include(c => c.ActorCharacters)
                    .ThenInclude(ac => ac.Character)
                .FirstOrDefaultAsync(c => c.ID == ActorID);

            if (actor == null)
            {
                return NotFound("No character was found");
            }

            Character character = new Character
            {
                Name = model.Name,
                Age = model.Age,
                Gender = model.Gender,
                PicturePath = model.PicturePath,
                Bio = model.Bio,
                Likes = 0,
                Dislikes = 0,
            };

            await _context.Characters.AddAsync(character);
            //This is important so we can get the ID
            await _context.SaveChangesAsync();

            ActorCharacter actorcharacter = new ActorCharacter
            {
                ActorID = actor.ID,
                CharacterID = character.ID
            };

            await _context.ActorCharacters.AddAsync(actorcharacter);
            //This line makes sure the actorcharacter relation was added to the class itself
            actor.ActorCharacters.Add(actorcharacter);
            await _context.SaveChangesAsync();

            return Ok($"{character.Name} was created for {actor.Name}");
        }

        [HttpPut("{ActorID},{CharacterID}")]
        public async Task<ActionResult<string>> AddActorToCharacter(int ActorID, int CharacterID)
        {
            var actor = await _context.Actors.FirstOrDefaultAsync(a => a.ID == ActorID);
            if (actor == null)
            {
                return NotFound("no actor was found");
            }
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.ID == CharacterID);
            if (character == null)
            {
                return NotFound("no character was found");
            }

            ActorCharacter actorCharacter = new ActorCharacter
            {
                ActorID = ActorID,
                CharacterID = character.ID
            };

            _context.ActorCharacters.Add(actorCharacter);
            actor.ActorCharacters.Add(actorCharacter);
            character.ActorCharacters.Add(actorCharacter);

            await _context.SaveChangesAsync();

            return Ok($"{actor.Name} was added to {character.Name}");
        }
    }
}