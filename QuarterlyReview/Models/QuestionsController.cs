using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Rendering;
using Microsoft.EntityFrameworkCore;

namespace QuarterlyReview.Models
{
    public class QuestionsController : Controller
    {
        private readonly QuarterlyReviewsContext _context;

        public QuestionsController(QuarterlyReviewsContext context)
        {
            _context = context;    
        }

        // GET: Questions
        public async Task<IActionResult> Index()
        {
            var quarterlyReviewsContext = _context.Questions.Include(q => q.Qt).Include(q => q.QtTypeNavigation).Include(q => q.R);
            return View(await quarterlyReviewsContext.ToListAsync());
        }

        // GET: Questions/Details/5
        public async Task<IActionResult> Details(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questions = await _context.Questions
                .Include(q => q.Qt)
                .Include(q => q.QtTypeNavigation)
                .Include(q => q.R)
                .SingleOrDefaultAsync(m => m.QId == id);
            if (questions == null)
            {
                return NotFound();
            }

            return View(questions);
        }

        // GET: Questions/Create
        public IActionResult Create()
        {
            ViewData["QtId"] = new SelectList(_context.QuestionTemplate, "QtId", "QType");
            ViewData["QtType"] = new SelectList(_context.QuestionTypeList, "QuestionType", "QuestionType");
            ViewData["RId"] = new SelectList(_context.Reviews, "RId", "DivId");
            return View();
        }

        // POST: Questions/Create
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Create([Bind("RtId,RId,QtId,QtOrder,QtType,QtQuestion,QId,Answer")] Questions questions)
        {
            if (ModelState.IsValid)
            {
                _context.Add(questions);
                await _context.SaveChangesAsync();
                return RedirectToAction("Index");
            }
            ViewData["QtId"] = new SelectList(_context.QuestionTemplate, "QtId", "QType", questions.QtId);
            ViewData["QtType"] = new SelectList(_context.QuestionTypeList, "QuestionType", "QuestionType", questions.QtType);
            ViewData["RId"] = new SelectList(_context.Reviews, "RId", "DivId", questions.RId);
            return View(questions);
        }

        // GET: Questions/Edit/5
        public async Task<IActionResult> Edit(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questions = await _context.Questions.SingleOrDefaultAsync(m => m.QId == id);
            if (questions == null)
            {
                return NotFound();
            }
            ViewData["QtId"] = new SelectList(_context.QuestionTemplate, "QtId", "QType", questions.QtId);
            ViewData["QtType"] = new SelectList(_context.QuestionTypeList, "QuestionType", "QuestionType", questions.QtType);
            ViewData["RId"] = new SelectList(_context.Reviews, "RId", "DivId", questions.RId);
            return View(questions);
        }

        // POST: Questions/Edit/5
        // To protect from overposting attacks, please enable the specific properties you want to bind to, for 
        // more details see http://go.microsoft.com/fwlink/?LinkId=317598.
        [HttpPost]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> Edit(int id, [Bind("RtId,RId,QtId,QtOrder,QtType,QtQuestion,QId,Answer")] Questions questions)
        {
            if (id != questions.QId)
            {
                return NotFound();
            }

            if (ModelState.IsValid)
            {
                try
                {
                    _context.Update(questions);
                    await _context.SaveChangesAsync();
                }
                catch (DbUpdateConcurrencyException)
                {
                    if (!QuestionsExists(questions.QId))
                    {
                        return NotFound();
                    }
                    else
                    {
                        throw;
                    }
                }
                return RedirectToAction("Index");
            }
            ViewData["QtId"] = new SelectList(_context.QuestionTemplate, "QtId", "QType", questions.QtId);
            ViewData["QtType"] = new SelectList(_context.QuestionTypeList, "QuestionType", "QuestionType", questions.QtType);
            ViewData["RId"] = new SelectList(_context.Reviews, "RId", "DivId", questions.RId);
            return View(questions);
        }

        // GET: Questions/Delete/5
        public async Task<IActionResult> Delete(int? id)
        {
            if (id == null)
            {
                return NotFound();
            }

            var questions = await _context.Questions
                .Include(q => q.Qt)
                .Include(q => q.QtTypeNavigation)
                .Include(q => q.R)
                .SingleOrDefaultAsync(m => m.QId == id);
            if (questions == null)
            {
                return NotFound();
            }

            return View(questions);
        }

        // POST: Questions/Delete/5
        [HttpPost, ActionName("Delete")]
        [ValidateAntiForgeryToken]
        public async Task<IActionResult> DeleteConfirmed(int id)
        {
            var questions = await _context.Questions.SingleOrDefaultAsync(m => m.QId == id);
            _context.Questions.Remove(questions);
            await _context.SaveChangesAsync();
            return RedirectToAction("Index");
        }

        private bool QuestionsExists(int id)
        {
            return _context.Questions.Any(e => e.QId == id);
        }
    }
}
