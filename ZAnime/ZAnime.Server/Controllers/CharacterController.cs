﻿using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Microsoft.EntityFrameworkCore;
using Zanime.Server.Data;
using Zanime.Server.Models.Main;
using Zanime.Server.Models.Main.DTO.Actor_Model;
using Zanime.Server.Models.Main.DTO.Character_Model;

namespace Zanime.Server.Controllers
{
    [Route("[controller]/[action]")]
    [ApiController]
    public class CharacterController : ControllerBase
    {
        private readonly ApplicationDbContext _context;

        public CharacterController(ApplicationDbContext context)
        {
            _context = context;
        }

        [HttpGet]
        public async Task<ActionResult<IEnumerable<Character>>> GetAll()
        {
            var characters = await _context.Characters.ToListAsync();
            return Ok(characters);
        }

        [HttpGet("{ID}")]
        public async Task<ActionResult<Character>> Get(int ID)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.ID == ID);
            if (character == null)
            {
                return NotFound("No character was found");
            }
            return Ok(character);
        }

        [HttpGet("{ID}")]
        public async Task<ActionResult<List<Actor>>> GetActors(int ID)
        {
            var character = await _context.Characters
                .Include(c => c.ActorCharacters)
                    .ThenInclude(ac => ac.Actor)
                .FirstOrDefaultAsync(c => c.ID == ID);

            if (character.ActorCharacters.Any())
            {
                var actors = character.ActorCharacters.Select(ac => new ActorVM
                {
                    Name = ac.Actor.Name,
                    Age = ac.Actor.Age,
                    Bio = ac.Actor.Bio,
                    Gender = ac.Actor.Gender,
                    PicturePath = ac.Actor.PicturePath
                }).ToList();
                return Ok(actors);
            }
            return Ok("No actors were found");
        }

        [HttpPost]
        public async Task<ActionResult<string>> Post(CharacterVM model)
        {
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

            if (await _context.Characters.AnyAsync(c => c.Name == character.Name))
            {
                return Conflict("Error");
            }

            await _context.Characters.AddAsync(character);
            await _context.SaveChangesAsync();

            return Ok($"{character.Name} was added ");
        }

        [HttpPut("{ID}")]
        public async Task<ActionResult<string>> Put(CharacterVM model, int ID)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.ID == ID);
            if (character == null)
            {
                return NotFound("No character was found");
            }

            character.Name = model.Name;
            character.Age = model.Age;
            character.Gender = model.Gender;
            character.PicturePath = model.PicturePath;
            character.Bio = model.Bio;

            await _context.SaveChangesAsync();

            return Ok("Character was modified");
        }

        [HttpDelete("{ID}")]
        public async Task<ActionResult<string>> Delete(int ID)
        {
            var character = await _context.Characters.FirstOrDefaultAsync(c => c.ID == ID);
            if (character == null)
            {
                return NotFound("No character was found");
            }
            _context.Characters.Remove(character);
            await _context.SaveChangesAsync();

            return Ok("Character was Deleted");
        }
    }
}