using Microsoft.AspNetCore.Identity.UI.Services;
using Microsoft.EntityFrameworkCore;
using Microsoft.VisualStudio.Web.CodeGenerators.Mvc.Templates.BlazorIdentity.Pages.Manage;
using NuGet.Protocol.Plugins;
using ProjectGraduation.Helping_Models;
using ProjectGraduation.Models;
using ProjectGraduation.Services;

namespace ProjectGraduation.Repository
{
    public class OfferRepository : IOfferRepository
    {
        private readonly GraduationProjectContext context;
        private readonly IEmailSender emailSender;
        public OfferRepository(GraduationProjectContext context , IEmailSender emailSender)
        {
            this.context = context;
            this.emailSender = emailSender;

        }
        public async Task<ProcessResult> AddOffer(OfferModel model)
        {
            try
            {
                await context.Offers.AddAsync(new Offer
                {
                    AddedAt = DateTime.UtcNow,
                    ClientId = model.ClientId,
                    Description = model.Description,
                    PostId = model.PostId
                });
                await context.SaveChangesAsync();
                return new ProcessResult { IsSucceded = true, Message = "Offer Added Successfully" };
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return new ProcessResult { IsSucceded = false, Message = "Error Adding Offer Due to " + msg };

            }
        }

        public async Task<ProcessResult> DeleteAllOffersOfPost(string PostId)
        {
            var Post = await context.Posts.FirstOrDefaultAsync(p => p.Id == PostId);
            if (Post == null)
                return new ProcessResult { IsSucceded = false, Message = "There is no post with id " + PostId };
            var Offers = await context.Offers.Where(O => O.PostId == PostId).ToListAsync();
            if (Offers != null)
            {
                try
                {
                    context.Offers.RemoveRange(Offers);
                    await context.SaveChangesAsync();
                    return new ProcessResult { IsSucceded = true, Message = "Offers Deleted Successfully" };
                }
                catch (Exception ex)
                {
                    string msg;
                    if (ex.InnerException != null)
                        msg = ex.InnerException.Message;
                    else
                        msg = ex.Message;
                    return new ProcessResult { IsSucceded = false, Message = "Error Deleting Offers due to "+msg };
                }
                
            }
            return new ProcessResult { IsSucceded = true, Message = "There is no Offers for Post" };
        }

        public async Task<ProcessResult> DeleteOffer(string OfferId)
        {
            var offer = await context.Offers.FirstOrDefaultAsync(O=>O.Id == OfferId);
            try
            {
                context.Offers.Remove(offer);
                await context.SaveChangesAsync();
                return new ProcessResult { IsSucceded = true, Message = "Offer Deleted Successfully" };
            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return new ProcessResult { IsSucceded = false, Message = "Error Deleting Offer due to " + msg };


            }
        }

        public async Task<List<Offer>> GetAllOffersOfPost(string PostId)
        {
            var offers = await context.Offers.Include(p=>p.Client).Where(p => p.PostId == PostId).OrderByDescending(p=>p.AddedAt).ToListAsync();
            return offers;
        }

        public async Task<Offer> GetOfferById(string OfferId)
        {
            var offer = await context.Offers.Include(O=>O.Client).FirstOrDefaultAsync(O=>O.Id == OfferId);
            return offer;
        }

        public async Task<ProcessResult> UpdateOffer(OfferModel model, string OfferId)
        {
            var offer = await context.Offers.FirstOrDefaultAsync(O=>O.Id ==OfferId);
            try
            {
                offer.Description = model.Description;
                offer.AddedAt = DateTime.Now;
                await context.SaveChangesAsync();
              return new ProcessResult { IsSucceded = true, Message ="Offer is Updated Successfully" };

            }
            catch (Exception ex)
            {
                string msg;
                if (ex.InnerException != null)
                    msg = ex.InnerException.Message;
                else
                    msg = ex.Message;
                return new ProcessResult { IsSucceded = false, Message = "Error Updating Offers due to " + msg };

            }
        }
        public async Task<ProcessResult> AcceptOffer(string OfferIId)
        {
            var result = await SendAcceptEmail(OfferIId);
            return result;
        }
        private async Task<ProcessResult> SendAcceptEmail(string OfferId)
        {
            var offer = await context.Offers.FirstOrDefaultAsync(p => p.Id == OfferId);
            var user = await context.Clients.FirstOrDefaultAsync(u => u.Id == offer.ClientId);
            var post = await context.Posts.FirstOrDefaultAsync(P => P.Id == offer.PostId);
            try
            {
                await emailSender.SendEmailAsync(user.Email, "Offer Acceptence", $"<center><h1>Your Offer on Post Offering {post.Title} Is Accepted</h1>\n" +
                  $"You can check it Via this Link<h1 style=\"color:blue\"><a href=\"https://moshref-gp.onrender.com/orders/{post.Id}\">Click Here</a></h1></center>");
                return new ProcessResult { IsSucceded = true, Message = "Email is sent Successfully" };
            }
            catch(Exception ex)
            {
                if (ex.InnerException != null)
                    return new ProcessResult { IsSucceded = false, Message = "Error Sending Email Due to " + ex.InnerException.Message };
                return new ProcessResult { IsSucceded = false, Message = "Error Sending Email Due to " + ex.Message };
            }

        }
    }
}
